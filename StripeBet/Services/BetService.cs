using StripeBet.Controllers;
using StripeBet.Data;
using StripeBet.Models;
using Microsoft.EntityFrameworkCore;
using Stripe.V2;

namespace StripeBet.Services
{
    public class BetService : IBetService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<BetService> _logger;

        public BetService(ApplicationDbContext context, IUserService userService, ILogger<BetService> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }
        public async Task<bool> ProcessBet(string userEmail, decimal betAmount, bool won)
        {
            var user = _userService.GetUserByEmail(userEmail);
            if (user == null || user.Balance < betAmount)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (won)
                {
                    user.Balance += betAmount;
                }
                else
                {
                    user.Balance -= betAmount;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing bet for user {userEmail}");
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task UpdateUserBets(string userEmail, BetResultViewModel betResult)
        {
            var user = _userService.GetUserByEmail(userEmail);
            if (user != null)
            {
                user.BetResults.Add(betResult);
                await _context.SaveChangesAsync();
            }
        }

        public bool CanBet(string userEmail, decimal amount)
        {
            var user = _userService.GetUserByEmail(userEmail);
            return user != null && user.Balance >= amount && amount > 0;
        }
    }
}
