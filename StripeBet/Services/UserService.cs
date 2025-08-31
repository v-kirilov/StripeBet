using StripeBet.Data;
using StripeBet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace StripeBet.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        public User? ValidateUser(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return null;

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
                return result == PasswordVerificationResult.Success ? user : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate user.");
                throw new InvalidOperationException($"Failed to validate user: {ex.Message}", ex);
            }

        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.Include(u => u.Transactions).Include(u => u.BetResults).FirstOrDefault(u => u.Email == email);
        }

        public async Task<User?> UpdateUserBalance(string userEmail, decimal amount)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user != null)
                {
                    user.Balance += amount;
                    await _context.SaveChangesAsync();
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user balance.");
                throw new InvalidOperationException($"Failed to update user balance: {ex.Message}", ex);
            }

        }



        public async Task<User?> CreateUser(string username, string email, string password)
        {
            try
            {
                if (EmailExists(email))
                {
                    return null;
                }

                var user = new User
                {
                    Username = username,
                    Email = email,
                    Password = _passwordHasher.HashPassword(null!, password),
                    Balance = 0.00m,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user.");
                throw new InvalidOperationException($"Failed to create user: {ex.Message}", ex);
            }
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
