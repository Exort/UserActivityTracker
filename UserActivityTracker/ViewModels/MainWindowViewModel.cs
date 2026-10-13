using System.Windows.Input;

namespace UserActivityTracker.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public UserFormViewModel UserForm { get; }
        public ActivityTypeFormViewModel ActivityForm { get; }
        public ActivityLogFormViewModel ActivityLogForm { get; }

        public MainWindowViewModel(UserFormViewModel userFormViewModel,
            ActivityTypeFormViewModel activityFormViewModel,
            ActivityLogFormViewModel activityLogFormViewModel)
        {
            UserForm = userFormViewModel;
            ActivityForm = activityFormViewModel;
            ActivityLogForm = activityLogFormViewModel;
        }

        public override async Task RefreshDataAsync()
        {
            await UserForm.RefreshDataAsync();
            await ActivityForm.RefreshDataAsync();
            await ActivityLogForm.RefreshDataAsync();
        }
    }
}