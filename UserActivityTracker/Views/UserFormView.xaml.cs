using System.Windows;
using System.Windows.Controls;
using UserActivityTracker.ViewModels;

namespace UserActivityTracker.Views
{
    public partial class UserFormView : UserControl
    {
        private bool _isUpdatingPasswordBox = false;
        public UserFormView()
        {
            InitializeComponent();
            IsVisibleChanged += ActivityLogFormView_IsVisibleChanged;
        }

        private void ActivityLogFormView_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ViewModel vm)
                vm.RefreshDataAsync().Wait();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingPasswordBox) return;

            if (DataContext is UserFormViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}