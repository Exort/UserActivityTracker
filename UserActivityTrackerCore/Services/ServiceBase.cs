using UserActivityTracker.Data;

namespace UserActivityTracker.Services
{
    public abstract class ServiceBase
    {
        protected readonly UserActivityTrackerDbContext _dbContext;

        protected ServiceBase(UserActivityTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
