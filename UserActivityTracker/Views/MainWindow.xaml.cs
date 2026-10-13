using System.Windows;
using System.Windows.Controls;
using UserActivityTracker.Data;
using UserActivityTracker.ViewModels;

namespace UserActivityTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            TabControl.SelectionChanged += TabControl_SelectionChanged;
            viewModel.RefreshDataAsync().Wait();
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem selectedTab && selectedTab.DataContext is ViewModel vm)
            {
                await vm.RefreshDataAsync();
            }
        }
    }
}