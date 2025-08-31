using StripeBet.Models;

namespace StripeBet.Services
{
    public interface IUserService
    {
        Task<User?> CreateUser(string username, string email, string password);
        bool EmailExists(string email);
        User? GetUserByEmail(string email);
        Task<User?> UpdateUserBalance(string userEmail, decimal amount);
        User? ValidateUser(string email, string password);
    }
}