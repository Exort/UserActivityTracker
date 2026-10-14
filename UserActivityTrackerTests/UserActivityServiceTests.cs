using UserActivityTracker.Services;
using UserActivityTrackerTests.Helpers;

namespace UserActivityTrackerTests.Tests
{
    [CollectionDefinition("UserActivityServiceTests", DisableParallelization = true)]
    public class UserActivityServiceTests
    {
        [Fact]
        public async Task CreateUserActivity_CreatesSuccessfully()
        {
            var dbContextScope = new TextDBContextScope();
            var userService = new UserService(dbContextScope.DbContext);
            var activityService = new ActivityService(dbContextScope.DbContext);
            var uaService = new UserActivityService(dbContextScope.DbContext, userService, activityService);

            var user = await userService.CreateUserAsync("u1", "AA", "BB", "password");
            var at = await activityService.CreateActivityTypeAsync("Jump");

            var created = await uaService.CreateUserActivityAsync(user.Id, at.Id, DateTime.UtcNow);
            Assert.True(created.Id > 0);

            var all = await uaService.GetAllUserActivitiesAsync();
            Assert.Contains(all, x => x.Id == created.Id && x.UserId == user.Id && x.ActivityTypeId == at.Id);
        }

        [Fact]
        public async Task CreateUserActivity_Throws_OnInvalidInput()
        {
            var dbContextScope = new TextDBContextScope();
            var userService = new UserService(dbContextScope.DbContext);
            var activityService = new ActivityService(dbContextScope.DbContext);
            var uaService = new UserActivityService(dbContextScope.DbContext, userService, activityService);

            await Assert.ThrowsAsync<ArgumentException>(async () => await uaService.CreateUserActivityAsync(-1, -1, DateTime.UtcNow));
        }

        [Fact]
        public async Task UpdateUserActivity_UpdatesFields()
        {
            var dbContextScope = new TextDBContextScope();
            var userService = new UserService(dbContextScope.DbContext);
            var activityService = new ActivityService(dbContextScope.DbContext);
            var uaService = new UserActivityService(dbContextScope.DbContext, userService, activityService);

            var user = await userService.CreateUserAsync("u1", "First", "Last", "password");
            var at1 = await activityService.CreateActivityTypeAsync("A1");
            var at2 = await activityService.CreateActivityTypeAsync("A2");

            var created = await uaService.CreateUserActivityAsync(user.Id, at1.Id, DateTime.UtcNow.AddDays(-1));
            created.ActivityTypeId = at2.Id;
            created.Date = DateTime.UtcNow;
            await uaService.UpdateUserActivityAsync(created);

            var filtered = await uaService.GetFilteredUserActivitiesAsync(user.Id, at2.Id, null, null);
            Assert.Single(filtered);
            Assert.Equal(at2.Id, filtered[0].ActivityTypeId);
        }

        [Fact]
        public async Task DeleteUserActivity_Removes()
        {
            var dbContextScope = new TextDBContextScope();
            var userService = new UserService(dbContextScope.DbContext);
            var activityService = new ActivityService(dbContextScope.DbContext);
            var uaService = new UserActivityService(dbContextScope.DbContext, userService, activityService);

            var user = await userService.CreateUserAsync("u1", "F", "L", "password");
            var at = await activityService.CreateActivityTypeAsync("TempAct");

            var created = await uaService.CreateUserActivityAsync(user.Id, at.Id, DateTime.UtcNow);
            await uaService.DeleteUserActivityAsync(created.Id);

            var all = await uaService.GetAllUserActivitiesAsync();
            Assert.DoesNotContain(all, x => x.Id == created.Id);
        }

        [Fact]
        public async Task GetFilteredUserActivities_FiltersCorrectly()
        {
            var dbContextScope = new TextDBContextScope();
            var userService = new UserService(dbContextScope.DbContext);
            var activityService = new ActivityService(dbContextScope.DbContext);
            var uaService = new UserActivityService(dbContextScope.DbContext, userService, activityService);

            var u1 = await userService.CreateUserAsync("u1", "A", "B", "p1");
            var u2 = await userService.CreateUserAsync("u2", "C", "D", "p2");
            var at1 = await activityService.CreateActivityTypeAsync("T1");
            var at2 = await activityService.CreateActivityTypeAsync("T2");

            var now = DateTime.UtcNow;
            await uaService.CreateUserActivityAsync(u1.Id, at1.Id, now.AddDays(-2));
            await uaService.CreateUserActivityAsync(u1.Id, at2.Id, now.AddDays(-1));
            await uaService.CreateUserActivityAsync(u2.Id, at1.Id, now);

            var byUser = await uaService.GetFilteredUserActivitiesAsync(u1.Id, null, null, null);
            Assert.Equal(2, byUser.Count);

            var byType = await uaService.GetFilteredUserActivitiesAsync(null, at1.Id, null, null);
            Assert.Equal(2, byType.Count);

            var byDateRange = await uaService.GetFilteredUserActivitiesAsync(null, null, now.AddDays(-1), now);
            Assert.Equal(2, byDateRange.Count);
        }
    }
}