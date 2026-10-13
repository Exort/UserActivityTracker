using System.Windows.Controls;
using UserActivityTracker.ViewModels;

namespace UserActivityTracker.Views
{
    public partial class ActivityTypeFormView : UserControl
    {
        public ActivityTypeFormView()
        {
            InitializeComponent();
            IsVisibleChanged += ActivityLogFormView_IsVisibleChanged;
        }

        private void ActivityLogFormView_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ViewModel vm)
                vm.RefreshDataAsync().Wait();
        }
    }
}