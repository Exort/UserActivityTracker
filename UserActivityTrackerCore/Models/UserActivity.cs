namespace UserActivityTracker.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; } = null!;
    }
}
