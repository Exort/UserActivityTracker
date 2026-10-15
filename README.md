# UserActivityTracker

A lightweight WPF desktop app for recording and querying users, activity types, and user activity logs. The project separates a WPF UI from a core library that contains the domain models and services so the core logic can be reused or tested independently.

## Stack
- Language(s): C#
- Framework / runtime: .NET 10 (WPF desktop)
- Target Framework: net10 (see project TargetFramework: net10.0-windows7.0)
- Notable libraries: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Sqlite, Microsoft.Extensions.Hosting

## Features
- Create, edit, and list users
- Define activity types
- Log user activities
- Clean separation of UI (UserActivityTracker) and business logic (UserActivityTrackerCore)
- Basic test project included (UserActivityTrackerTests)

## How it fits together
- The WPF application (UserActivityTracker) contains Views (.xaml) and ViewModels which implement UI behavior and data-binding.
- ViewModels call into the services in UserActivityTrackerCore to perform domain operations (create/list/update users, activity types, and activities).
- UserActivityTrackerCore contains domain models (User, UserActivity, ActivityType) and service classes (UserService, UserActivityService, ActivityService) to encapsulate business logic and persistence concerns. This keeps UI and domain logic decoupled and testable.

## Getting started — build & run

### Prerequisites
- .NET 10 SDK
- On Windows: Visual Studio 2026 (for the WPF UI) or the dotnet CLI

### From a fresh clone (CLI)
```bash
dotnet restore
dotnet build
dotnet run --project UserActivityTracker/UserActivityTracker.csproj
```

### Run tests
```bash
dotnet test
# or run a specific test project
dotnet test UserActivityTrackerTests/UserActivityTrackerTests.csproj
```

### Build & launch with Visual Studio 2026
1. Open Visual Studio 2026.
2. Choose "Open > Project/Solution" and open the solution file `UserActivityTracker.slnx`.
3. In Solution Explorer, right-click the `UserActivityTracker` project and select "Set as Startup Project".
4. Press F5 (Start Debugging) or Ctrl+F5 (Start Without Debugging) to build and launch the WPF application.
5. Use the Test Explorer to run the tests from `UserActivityTrackerTests`.

## Configuration
- The app includes `appsettings.json` (UserActivityTracker/appsettings.json) for runtime configuration — adjust it as needed.
- The UI project references EF Core and SQLite packages; if you change persistence, update the core services accordingly.

## Development notes
- UI: UserActivityTracker/Views (XAML) and UserActivityTracker/ViewModels; RelayCommand.cs provides a lightweight ICommand implementation.
- Core: UserActivityTrackerCore/Models and UserActivityTrackerCore/Services contain domain models and services (User, UserActivity, ActivityType and their service classes).
- Keep domain logic in UserActivityTrackerCore to allow reuse by other hosts (CLI, web API, etc.).
