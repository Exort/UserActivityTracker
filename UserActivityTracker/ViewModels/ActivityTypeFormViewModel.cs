using System.Collections.ObjectModel;
using System.Windows.Input;
using UserActivityTracker.Models;
using UserActivityTracker.Services;

namespace UserActivityTracker.ViewModels
{
    public class ActivityTypeFormViewModel : ViewModel
    {
        private readonly ActivityService _activityService;

        public ObservableCollection<ActivityType> ActivityTypes { get; set; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private ActivityType? _selectedActivityType;
        public ActivityType? SelectedActivityType
        {
            get => _selectedActivityType;
            set
            {
                _selectedActivityType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditMode));

                if (_selectedActivityType != null)
                {
                    Name = _selectedActivityType.Name;
                }
                else
                {
                    ClearFormFields();
                }
            }
        }

        public bool IsEditMode => SelectedActivityType != null;

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DeleteCommand { get; }

        public ActivityTypeFormViewModel(ActivityService activityService)
        {
            _activityService = activityService;
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => IsEditMode);
        }

        public override async Task RefreshDataAsync()
        {
            ActivityTypes.Clear();
            var list = await _activityService.SearchActivityTypesAsync(SearchText);
            foreach (var item in list)
            {
                ActivityTypes.Add(item);
            }
        }

        private async Task SaveAsync()
        {
            if (SelectedActivityType == null)
            {
                await _activityService.CreateActivityTypeAsync(Name);
            }
            else
            {
                SelectedActivityType.Name = Name;
                await _activityService.UpdateActivityTypeAsync(SelectedActivityType);
            }
            Clear();
            await RefreshDataAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedActivityType != null)
            {
                await _activityService.DeleteActivityTypeAsync(SelectedActivityType.Id);
                Clear();
                await RefreshDataAsync();
            }
        }

        public void Clear()
        {
            SelectedActivityType = null;
            ClearFormFields();
        }

        private void ClearFormFields()
        {
            Name = string.Empty;
        }
    }
}