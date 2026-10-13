namespace UserActivityTracker.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        internal int UserId { get; set; }
        public User User { get; set; } = null!;

        internal int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; } = null!;
    }
}
