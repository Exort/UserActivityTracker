using Microsoft.EntityFrameworkCore;
using UserActivityTracker.Data;
using UserActivityTracker.Models;

namespace UserActivityTracker.Services
{
    public class UserActivityService
    {
        private readonly UserActivityTrackerDbContext _dbContext;
        private readonly UserService _userService;
        private readonly ActivityService _activityService;

        public UserActivityService(UserActivityTrackerDbContext dbContext, UserService userService, ActivityService activityService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _activityService = activityService;
        }

        public async Task<List<UserActivity>> GetAllUserActivitiesAsync()
        {
            return await _dbContext.UserActivities
                .Include(ua => ua.User)
                .Include(ua => ua.ActivityType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserActivity> CreateUserActivityAsync(int userId, int activityTypeId, DateTime activityDate)
        {
            if (userId < 0 || activityTypeId < 0)
            {
                throw new ArgumentException("User Activity details cannot be null or empty.");
            }

            var userActivity = new UserActivity
            {
                UserId = userId,
                ActivityTypeId = activityTypeId,
                Date = activityDate
            };

            _dbContext.UserActivities.Add(userActivity);
            await _dbContext.SaveChangesAsync();
            return userActivity;
        }

        public async Task UpdateUserActivityAsync(UserActivity userActivity)
        {
            _dbContext.UserActivities.Update(userActivity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserActivityAsync(int id)
        {
            var userActivity = await _dbContext.UserActivities.FindAsync(id);
            if (userActivity != null)
            {
                _dbContext.UserActivities.Remove(userActivity);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retrieves user activities filtered by user, activity type, and/or date range.
        /// </summary>
        public async Task<List<UserActivity>> GetFilteredUserActivitiesAsync(
            int? userId, int? activityTypeId, DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.UserActivities
                .Include(ua => ua.User)
                .Include(ua => ua.ActivityType)
                .AsNoTracking()
                .AsQueryable();

            if (userId.HasValue && userId.Value > 0)
            {
                query = query.Where(ua => ua.UserId == userId.Value);
            }

            if (activityTypeId.HasValue && activityTypeId.Value > 0)
            {
                query = query.Where(ua => ua.ActivityTypeId == activityTypeId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(ua => ua.Date.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(ua => ua.Date.Date <= endDate.Value.Date);
            }

            return await query.OrderByDescending(ua => ua.Date).ToListAsync();
        }
    }
}