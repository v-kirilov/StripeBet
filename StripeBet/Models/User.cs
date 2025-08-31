using System.ComponentModel.DataAnnotations;

namespace StripeBet.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public List<BetResultViewModel> BetResults { get; set; } = new List<BetResultViewModel>();
    }
}
