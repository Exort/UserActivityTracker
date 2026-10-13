using System.Collections.ObjectModel;
using System.Windows.Input;
using UserActivityTracker.Models;
using UserActivityTracker.Services;

namespace UserActivityTracker.ViewModels
{
    public class UserFormViewModel : ViewModel
    {
        private readonly UserService _userService;

        // Collection of users available for selection/search
        public ObservableCollection<User> Users { get; set; } = new();

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

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditMode));

                if (_selectedUser != null)
                {
                    UserName = _selectedUser.UserName;
                    FirstName = _selectedUser.FirstName;
                    LastName = _selectedUser.LastName;
                    Password = string.Empty; // Don't expose password hashes
                    _passwordModified = false;
                }
                else
                {
                    ClearFormFields();
                }
            }
        }

        public bool IsEditMode => SelectedUser != null;

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }
        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        private bool _passwordModified = false;
        public string Password
        {
            get => _password;
            set 
            { 
                _password = value;
                _passwordModified = true;
                OnPropertyChanged(); 
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DeleteCommand { get; }

        public UserFormViewModel(UserService userService)
        {
            _userService = userService;
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => IsEditMode);
        }

        public override async Task RefreshDataAsync()
        {
            Users.Clear();
            var filteredUsers = string.IsNullOrEmpty(SearchText) 
                ? await _userService.GetAllUsersAsync()
                : await _userService.SearchUsersAsync(SearchText);
            foreach(var user in filteredUsers)
            {
                Users.Add(user);
            }
        }

        private async Task SaveAsync()
        {
            if (SelectedUser == null)
            {
                await _userService.CreateUserAsync(UserName, FirstName, LastName, Password);
            }
            else
            {
                SelectedUser.FirstName = UserName;
                SelectedUser.FirstName = FirstName;
                SelectedUser.LastName = LastName;
                if (_passwordModified)
                {
                    SelectedUser.SaltedPassword = Password;
                }
                await _userService.UpdateUserAsync(SelectedUser);
            }

            Clear();
            await RefreshDataAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedUser != null)
            {
                await _userService.DeleteUserByIdAsync(SelectedUser.Id);
                Clear();
                await RefreshDataAsync();
            }
        }

        public void Clear()
        {
            SelectedUser = null;
            ClearFormFields();
        }

        private void ClearFormFields()
        {
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;
            _passwordModified = false;
        }
    }
}