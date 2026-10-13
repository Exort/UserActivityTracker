using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UserActivityTracker.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
        }

        public abstract Task RefreshDataAsync();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
