using System.IO;
using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserActivityTracker.Data;
using UserActivityTracker.Services;
using UserActivityTracker.ViewModels;
using UserActivityTracker.Views;

namespace UserActivityTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Ensure configuration looks in the application's base directory
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // Read connection string from configuration
                    var connectionString = configuration.GetConnectionString("DefaultConnection")
                                           ?? "Data Source=app.db";

                    var sqlListConnection = new SqliteConnection(connectionString);
                    services.AddSingleton(sqlListConnection);

                    // Register AppDbContext with DI
                    services.AddDbContext<UserActivityTrackerDbContext>(options =>
                        options.UseSqlite(sqlListConnection));

                    // Services
                    services.AddTransient<UserService>();
                    services.AddTransient<ActivityService>();
                    services.AddTransient<UserActivityService>();

                    // ViewModels
                    services.AddTransient<UserFormViewModel>();
                    services.AddTransient<ActivityTypeFormViewModel>();
                    services.AddTransient<ActivityLogFormViewModel>();
                    services.AddTransient<MainWindowViewModel>();

                    // MainWindow
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Ensure the SQLite database file and tables are created on startup
            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserActivityTrackerDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Resolve and show the MainWindow via DI
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}
