using System;
using System.Reflection;
using AutoMapper;
using Backend.Users.Commands.Common;
using Backend.Users.DataAccess;
using Backend.Users.Domain.Aggregates.UserAggregate;
using Backend.Users.Domain.Services;
using Backend.Users.Queries.Common;
using Backend.Users.Services.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backend.Web.Users
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
            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.MaxBatchSize(50000);
                        sql.MigrationsAssembly(typeof(UsersDbContext).GetTypeInfo().Assembly.FullName);
                    });
            });
            services.AddMvc();
            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = false;
                options.LowercaseUrls = true;
            });
            services.AddAutoMapper();
            services.AddMediatR(
                typeof(BackendUsersCommandsMarker),
                typeof(BackendUsersQueriesMarker));
            services.AddScoped<UserFactory>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPasswordValidator, PasswordHasher>();
            services.Configure<PasswordHasherOptions>(_configuration.GetSection("PasswordHasherOptions"));
            services.AddScoped(resolver =>
            {
                var snapshot = resolver.GetRequiredService<IOptionsSnapshot<PasswordHasherOptions>>();
                return snapshot.Value;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvcWithDefaultRoute();
        }
    }
}