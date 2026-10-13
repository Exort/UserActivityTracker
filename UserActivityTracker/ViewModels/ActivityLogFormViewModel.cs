using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UserActivityTracker.Models;
using UserActivityTracker.Services;

namespace UserActivityTracker.ViewModels
{
    public class ActivityLogFormViewModel : ViewModel
    {
        private readonly UserActivityService _userActivityService;
        private readonly UserService _userService;
        private readonly ActivityService _activityService;

        public ObservableCollection<UserActivity> ActivityLogs { get; set; } = new();
        public ObservableCollection<User> Users { get; set; } = new();
        public ObservableCollection<ActivityType> ActivityTypes { get; set; } = new();

        // --- Filter Properties ---
        private User? _filterUser;
        public User? FilterUser
        {
            get => _filterUser;
            set { _filterUser = value; OnPropertyChanged(); _ = LoadLogsAsync(); }
        }

        private ActivityType? _filterActivityType;
        public ActivityType? FilterActivityType
        {
            get => _filterActivityType;
            set { _filterActivityType = value; OnPropertyChanged(); _ = LoadLogsAsync(); }
        }

        private DateTime? _filterStartDate;
        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set { _filterStartDate = value; OnPropertyChanged(); _ = LoadLogsAsync(); }
        }

        private DateTime? _filterEndDate;
        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set { _filterEndDate = value; OnPropertyChanged(); _ = LoadLogsAsync(); }
        }

        // --- Form Properties ---
        private UserActivity? _selectedActivityLog;
        public UserActivity? SelectedActivityLog
        {
            get => _selectedActivityLog;
            set
            {
                _selectedActivityLog = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditMode));

                if (_selectedActivityLog != null)
                {
                    SelectedUser = Users.FirstOrDefault(u => u.Id == _selectedActivityLog.UserId);
                    SelectedActivityType = ActivityTypes.FirstOrDefault(a => a.Id == _selectedActivityLog.ActivityTypeId);
                    Date = _selectedActivityLog.Date;
                }
                else
                {
                    ClearFormFields();
                }
            }
        }

        public bool IsEditMode => SelectedActivityLog != null;

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private ActivityType? _selectedActivityType;
        public ActivityType? SelectedActivityType
        {
            get => _selectedActivityType;
            set { _selectedActivityType = value; OnPropertyChanged(); }
        }

        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public ActivityLogFormViewModel(UserActivityService userActivityService, UserService userService, ActivityService activityService)
        {
            _userActivityService = userActivityService;
            _userService = userService;
            _activityService = activityService;

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => SelectedUser != null && SelectedActivityType != null);
            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => IsEditMode);
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
        }

        public override async Task RefreshDataAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            Users.Clear();
            foreach (var u in users) Users.Add(u);

            var types = await _activityService.GetAllActivityTypesAsync();
            ActivityTypes.Clear();
            foreach (var t in types) ActivityTypes.Add(t);

            await LoadLogsAsync();
        }

        public async Task LoadLogsAsync()
        {
            ActivityLogs.Clear();
            int? userId = FilterUser?.Id;
            int? activityTypeId = FilterActivityType?.Id;

            var logs = await _userActivityService.GetFilteredUserActivitiesAsync(
                userId, activityTypeId, FilterStartDate, FilterEndDate);

            foreach (var log in logs)
            {
                ActivityLogs.Add(log);
            }
        }

        private void ClearFilters()
        {
            _filterUser = null;
            _filterActivityType = null;
            _filterStartDate = null;
            _filterEndDate = null;
            OnPropertyChanged(nameof(FilterUser));
            OnPropertyChanged(nameof(FilterActivityType));
            OnPropertyChanged(nameof(FilterStartDate));
            OnPropertyChanged(nameof(FilterEndDate));
            _ = LoadLogsAsync();
        }

        private async Task SaveAsync()
        {
            if (SelectedUser == null || SelectedActivityType == null) return;

            if (SelectedActivityLog == null)
            {
                await _userActivityService.CreateUserActivityAsync(SelectedUser.Id, SelectedActivityType.Id, Date);
            }
            else
            {
                SelectedActivityLog.UserId = SelectedUser.Id;
                SelectedActivityLog.ActivityTypeId = SelectedActivityType.Id;
                SelectedActivityLog.Date = Date;
                await _userActivityService.UpdateUserActivityAsync(SelectedActivityLog);
            }
            Clear();
            await LoadLogsAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedActivityLog != null)
            {
                await _userActivityService.DeleteUserActivityAsync(SelectedActivityLog.Id);
                Clear();
                await LoadLogsAsync();
            }
        }

        public void Clear()
        {
            SelectedActivityLog = null;
            ClearFormFields();
        }

        private void ClearFormFields()
        {
            SelectedUser = null;
            SelectedActivityType = null;
            Date = DateTime.Now;
        }
    }
}