using System;
using System.Linq;
using Backend.Common.Users.Client;
using Backend.Identity.Services;
using Backend.Web.Identity.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backend.Web.Identity
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var config = _configuration.GetSection("IdentityServer").Get<IdentityServerConfiguration>();
            if (!config.IsValid())
                throw new Exception("Указаны некорректные настройки IdentityServer");
            var cert = config.SigningCertificate.ToCertificate();
            services.Configure<IdentityServerConfiguration>(_configuration.GetSection("IdentityServer"));
            services.AddIdentityServer()
                .AddOperationalStore(
                    options =>
                    {
                        var migrationsAssembly = typeof(Startup).Assembly.FullName;
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.MaxBatchSize(50000);
                            });
                        };
                    }
                )
                .AddConfigurationStore(
                    options =>
                    {
                        var migrationsAssembly = typeof(Startup).Assembly.FullName;
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.MaxBatchSize(50000);
                            });
                        };
                    }
                )
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddSigningCredential(cert);
            services.Configure<UsersApiClientOptions>(_configuration.GetSection("UsersApiClientOptions"));
            services.AddScoped(resolver =>
            {
                var snapshot = resolver.GetRequiredService<IOptionsSnapshot<UsersApiClientOptions>>();
                return snapshot.Value;
            });
            services.AddScoped<UsersApiClient>();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (env.EnvironmentName?.ToLowerInvariant()?.Contains("ef") == false)
                {
                    var config = scope
                        .ServiceProvider
                        .GetRequiredService<IOptionsSnapshot<IdentityServerConfiguration>>().Value;
                    var configContext = scope
                        .ServiceProvider
                        .GetRequiredService<ConfigurationDbContext>();
                    var persistContext = scope
                        .ServiceProvider
                        .GetRequiredService<PersistedGrantDbContext>();

                    persistContext.Database.Migrate();
                    configContext.Database.Migrate();

                    if (!configContext.ApiResources.Any())
                        configContext.ApiResources.AddRange(config.ApiResources.Select(x => x.ToEntity()));
                    if (!configContext.IdentityResources.Any())
                        configContext.IdentityResources.AddRange(config.IdentityResources.Select(x => x.ToEntity()));
                    if (!configContext.Clients.Any())
                        configContext.Clients.AddRange(config.Clients.Select(x => x.ToEntity()));
                    configContext.SaveChanges();
                }
            }

            app.UseIdentityServer();
        }
    }
}