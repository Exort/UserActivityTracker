using Microsoft.EntityFrameworkCore;
using UserActivityTracker.Data;
using UserActivityTracker.Models;

namespace UserActivityTracker.Services
{
    public class ActivityService
    {
        private readonly UserActivityTrackerDbContext _dbContext;

        public ActivityService(UserActivityTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ActivityType>> GetAllActivityTypesAsync()
        {
            return await _dbContext.ActivityTypes.AsNoTracking().ToListAsync();
        }

        public async Task<List<ActivityType>> SearchActivityTypesAsync(string searchTerm)
        {
            var query = _dbContext.ActivityTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(searchTerm));
            }

            return await query.ToListAsync();
        }

        public async Task<ActivityType> CreateActivityTypeAsync(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Activity type name cannot be null or empty.", nameof(name));
            }

            var activityType = new ActivityType
            {
                Name = name,
            };

            _dbContext.ActivityTypes.Add(activityType);
            await _dbContext.SaveChangesAsync();
            return activityType;
        }

        public async Task UpdateActivityTypeAsync(ActivityType activityType)
        {
            _dbContext.ActivityTypes.Update(activityType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActivityTypeAsync(int id)
        {
            var activityType = await _dbContext.ActivityTypes.FindAsync(id);
            if (activityType != null)
            {
                _dbContext.ActivityTypes.Remove(activityType);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}