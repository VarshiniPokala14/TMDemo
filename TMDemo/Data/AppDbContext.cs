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
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDetail>()
               .ToTable("Users")
               .HasOne(u => u.EmergencyContact)
               .WithOne(e => e.UserDetail)
               .OnDelete(DeleteBehavior.Cascade);
            //Configure EmergencyContact table
            modelBuilder.Entity<EmergencyContact>()
                .ToTable("EmergencyContacts")
                .HasOne(e => e.UserDetail) // Define relationship
                .WithOne(u => u.EmergencyContact) // Add navigation property
                .HasForeignKey<EmergencyContact>(e => e.UserId);

        }
    }
}
