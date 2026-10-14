using Microsoft.Data.Sqlite;
using UserActivityTracker.Data;

namespace UserActivityTrackerTests.Helpers
{
    public class TextDBContextScope : IDisposable
    {
        public UserActivityTrackerDbContext DbContext { get; }

        private readonly SqliteConnection _connection;

        public TextDBContextScope()
        {
            // Use an in-memory SQLite database per test. Keep the connection open for the
            // lifetime of the scope so the in-memory database is preserved.
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            DbContext = new UserActivityTrackerDbContext(_connection);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}