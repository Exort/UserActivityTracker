namespace UserActivityTracker.Models
{
    public class ActivityType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for EF Core
        public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
    }
}
