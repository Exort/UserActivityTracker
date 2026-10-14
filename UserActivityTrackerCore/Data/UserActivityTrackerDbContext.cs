using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UserActivityTracker.Models;

namespace UserActivityTracker.Data
{
    public class UserActivityTrackerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ActivityType> ActivityTypes { get; set; } = null!;
        public DbSet<UserActivity> UserActivities { get; set; } = null!;

        private readonly SqliteConnection _sqlLiteConnection;

        public UserActivityTrackerDbContext(SqliteConnection sqlLiteConnection)
        {
            _sqlLiteConnection = sqlLiteConnection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_sqlLiteConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships
            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.ActivityType)
                .WithMany(at => at.UserActivities)
                .HasForeignKey(ua => ua.ActivityTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}