using Backend.Users.DataAccess.Configurations;
using Backend.Users.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Backend.Users.DataAccess
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}