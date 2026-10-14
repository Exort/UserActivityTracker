using UserActivityTracker.Services;
using UserActivityTrackerTests.Helpers;

namespace UserActivityTrackerTests.Tests
{
    [CollectionDefinition("ActivityServiceTests", DisableParallelization = true)]
    public class ActivityServiceTests
    {
        [Fact]
        public async Task Can_Create_ActivityType_Successfully()
        {
            var dbContextScope = new TextDBContextScope();
            var service = new ActivityService(dbContextScope.DbContext);

            var at = await service.CreateActivityTypeAsync("Running");
            Assert.True(at.Id > 0);

            var all = await service.GetAllActivityTypesAsync();
            Assert.Contains(all, a => a.Id == at.Id && a.Name == "Running");
        }

        [Fact]
        public async Task CreateActivityType_Throws_OnInvalidInput()
        {
            var dbContextScope = new TextDBContextScope();
            var service = new ActivityService(dbContextScope.DbContext);

            await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateActivityTypeAsync(""));
        }

        [Fact]
        public async Task UpdateActivityType_UpdatesFields()
        {
            var dbContextScope = new TextDBContextScope();
            var service = new ActivityService(dbContextScope.DbContext);

            var at = await service.CreateActivityTypeAsync("Walk");
            at.Name = "Walk Fast";
            await service.UpdateActivityTypeAsync(at);

            var all = await service.GetAllActivityTypesAsync();
            Assert.Contains(all, a => a.Id == at.Id && a.Name == "Walk Fast");
        }

        [Fact]
        public async Task DeleteActivityType_Removes()
        {
            var dbContextScope = new TextDBContextScope();
            var service = new ActivityService(dbContextScope.DbContext);

            var at = await service.CreateActivityTypeAsync("Temp");
            await service.DeleteActivityTypeAsync(at.Id);

            var all = await service.GetAllActivityTypesAsync();
            Assert.DoesNotContain(all, a => a.Id == at.Id);
        }

        [Fact]
        public async Task SearchActivityTypes_FindsByName_AndEmptyReturnsAll()
        {
            var dbContextScope = new TextDBContextScope();
            var service = new ActivityService(dbContextScope.DbContext);

            await service.CreateActivityTypeAsync("Swimming");
            await service.CreateActivityTypeAsync("Cycling");

            var found = await service.SearchActivityTypesAsync("swim");
            Assert.Single(found);

            var all = await service.SearchActivityTypesAsync("");
            Assert.Equal(2, all.Count);
        }
    }
}
