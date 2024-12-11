using Microsoft.EntityFrameworkCore;
using TrekMasters.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace TrekMasters.Data
{
    public class AppDbContext : IdentityDbContext<UserDetail> 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<Trek> Treks { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<TrekReview> TrekReviews { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TrekPlan> TrekPlans { get; set; }
        public DbSet<NotificationRequest> NotificationRequests { get; set; }
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
            modelBuilder.Entity<TrekReview>()
                .ToTable("TrekReviews")
                .HasOne(tr => tr.User)
                .WithMany(u => u.TrekReviews)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrekReview>()
                .HasOne(tr => tr.Trek)
                .WithMany(t => t.TrekReviews)
                .HasForeignKey(tr => tr.TrekId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for one review per user per trek
            modelBuilder.Entity<TrekReview>()
                .HasIndex(tr => new { tr.UserId, tr.TrekId })
                .IsUnique();
            modelBuilder.Entity<Booking>()
                .ToTable("Bookings")
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Trek)
                .WithMany()
                .HasForeignKey(b => b.TrekId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Trek>()
                .HasMany(t => t.TrekPlans)
                .WithOne(tp => tp.Trek)
                .HasForeignKey(tp => tp.TrekId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<NotificationRequest>()
                .ToTable("NotificationRequests")
                .HasKey(nr => nr.NotificationRequestId);

        }
    }
}
