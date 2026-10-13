namespace UserActivityTracker.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string SaltedPassword { get; set; } = string.Empty;

        // Navigation property for EF Core
        public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
    }
}
