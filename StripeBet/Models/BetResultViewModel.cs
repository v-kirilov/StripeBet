namespace StripeBet.Models
{
    public class BetResultViewModel
    {
        public int Id { get; set; }
        public decimal BetAmount { get; set; }
        public BetColor BetColor { get; set; }
        public int RandomNumber { get; set; }
        public BetColor ResultColor { get; set; } 
        public bool Won { get; set; }
        public decimal Winnings { get; set; }
        public decimal NewBalance { get; set; }
    }
}