using Microsoft.EntityFrameworkCore;
using TMDemo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TMDemo.Data
{
    public class AppDbContext : IdentityDbContext<UserDetail> 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDetail>()
               .ToTable("Users");

        }
    }
}
