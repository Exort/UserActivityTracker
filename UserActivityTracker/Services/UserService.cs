using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using UserActivityTracker.Data;
using UserActivityTracker.Models;
using UserActivityTracker.Utils;

namespace UserActivityTracker.Services
{
    public class UserService
    {
        public event Action<User?>? LoggedUserChanged;

        private UserActivityTrackerDbContext _dbContext;

        public UserService(UserActivityTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User? _currentLoggedUser;
        public User? CurrentLoggedUser
        {
            get => _currentLoggedUser;
            private set
            {
                if(_currentLoggedUser != value)
                {
                    _currentLoggedUser = value;
                    LoggedUserChanged?.Invoke(CurrentLoggedUser);
                }
            }
        }

        public async Task<User> CreateUserAsync(string userName, string firstName, string lastName, string password)
        {
            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("User details cannot be null or empty.");
            }

            var newUser = new User
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                SaltedPassword = PasswordUtils.GetSaltedPassword(password)
            };

            // Add the entity to the context's Users collection
            _dbContext.Users.Add(newUser);

            // Commit changes to the SQLite database asynchronously
            int rowsAffected = await _dbContext.SaveChangesAsync();
            Trace.Assert(rowsAffected == 1);
            await _dbContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
                return false;

            //This could be improved by adding error reporting instead of just returning false
            if (user == CurrentLoggedUser)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            // Tells EF Core that this entity already exists and has been modified
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> AuthenticateAsync(string userName, string password)
        {
            User? foundUser = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName);
            if (foundUser != null && PasswordUtils.VerifyPassword(password, foundUser.SaltedPassword))
            {
                CurrentLoggedUser = foundUser;
            }

            return CurrentLoggedUser;
        }

        public void LogoutCurrentUser()
        {
            CurrentLoggedUser = null;
        }

        /// <summary>
        /// Searches for users matching a term against FirstName or LastName.
        /// </summary>
        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            IQueryable<User> query = _dbContext.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u => u.FirstName.ToLower().Contains(searchTerm) ||
                                         u.LastName.ToLower().Contains(searchTerm));
            }

            var results = await query.ToListAsync();
            return results.ToList();
        }
    }
}
