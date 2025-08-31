using System.ComponentModel.DataAnnotations;

namespace StripeBet.Models
{
    public class BetViewModel
    {
        [Required(ErrorMessage = "Bet amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bet amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please choose a color")]
        public BetColor BetColor { get; set; }

        public decimal CurrentBalance { get; set; }
    }
}