using StripeBet.Models;

namespace StripeBet.Services
{
    public interface IBetService
    {
        bool CanBet(string userEmail, decimal amount);
        Task<bool> ProcessBet(string userEmail, decimal betAmount, bool won);
        Task UpdateUserBets(string userEmail, BetResultViewModel betResult);
    }
}