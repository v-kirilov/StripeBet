using StripeBet.Models;
using Stripe;
using Stripe.Checkout;

namespace StripeBet.Services
{
    public interface IStripeService
    {
        Task<Session> BuyAsync(User user, DefaultProduct product);
        Task<PaymentIntent> ConfirmPaymentIntentAsync(PaymentIntent paymentIntent, string paymentMethod);
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency = "usd");
        Task<Payout> CreatePayoutIntentAsync(decimal amount, string currency = "usd");
        Task<Transaction> CreateTransaction(User user, decimal amount);
    }
}