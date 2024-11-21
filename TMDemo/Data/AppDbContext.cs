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
        public DbSet<Trek> Treks { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDetail>()
               .ToTable("Users")
               .HasOne(u => u.EmergencyContact)
               .WithOne(e => e.UserDetail)
               .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<EmergencyContact>()
                .ToTable("EmergencyContacts")
                .HasOne(e => e.UserDetail) 
                .WithOne(u => u.EmergencyContact) 
                .HasForeignKey<EmergencyContact>(e => e.UserId);
            modelBuilder.Entity<Trek>()
                .ToTable("Treks")
                .HasMany(t=>t.Availabilities)
                .WithOne(a => a.Trek)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Availability>()
                .ToTable("Availabilities")
                .HasOne(a => a.Trek)
                .WithMany(t => t.Availabilities)
                .HasForeignKey(a => a.TrekId);
                

        }
    }
}
