using System.Windows.Controls;
using UserActivityTracker.ViewModels;

namespace UserActivityTracker.Views
{
    public partial class ActivityLogFormView : UserControl
    {
        public ActivityLogFormView()
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