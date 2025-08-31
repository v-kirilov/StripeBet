using StripeBet.Data;
using StripeBet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.Forwarding;
using static System.Net.WebRequestMethods;

namespace StripeBet.Services
{


    public class StripeService : IStripeService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly CustomerService _customerService;
        private readonly PayoutService _payoutService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StripeService> _logger;

        public StripeService(ApplicationDbContext context, ILogger<StripeService> logger)
        {
            _paymentIntentService = new PaymentIntentService();
            _customerService = new CustomerService();
            _payoutService = new PayoutService();
            _logger = logger;
            _context = context;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency = "usd")
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100), // Convert to cents
                    Currency = currency,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                };

                return await _paymentIntentService.CreateAsync(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment intent");
                throw new InvalidOperationException($"Failed to create payment intent: {ex.Message}", ex);
            }
        }

        public async Task<PaymentIntent> ConfirmPaymentIntentAsync(PaymentIntent paymentIntent, string paymentMethod)
        {
            try
            {
                var options = new PaymentIntentConfirmOptions
                {
                    ReturnUrl = "https://localhost:7212",
                    PaymentMethod = paymentMethod,
                };

                return await _paymentIntentService.ConfirmAsync(paymentIntent.Id, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to confirm payment intent");
                throw new InvalidOperationException($"Failed to confirm payment intent: {ex.Message}", ex);
            }
        }

        public async Task<Payout> CreatePayoutIntentAsync(decimal amount, string currency = "usd")
        {
            try
            {
                var options = new PayoutCreateOptions { Amount = (long)(amount * 100), Currency = currency };


                return await _payoutService.CreateAsync(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payout intent");
                throw new InvalidOperationException($"Failed to create payout intent: {ex.Message}", ex);
            }
        }

        public async Task<Session> BuyAsync(User user, DefaultProduct product)
        {
            try
            {
                var striSessionService = new SessionService();
                var stripeCheckoutSession = await striSessionService.CreateAsync(new SessionCreateOptions
                {
                    Mode = "payment",
                    ClientReferenceId = Guid.NewGuid().ToString(),
                    SuccessUrl = "https://localhost:7212",
                    CancelUrl = "https://localhost:7212",
                    CustomerEmail = user.Email,
                    LineItems = new()
                {
                    new()
                    {
                        PriceData = new()
                        {
                            Currency = "usd",
                            ProductData = new()
                            {
                                Name = product.Name,
                            },
                            UnitAmountDecimal = product.Price * 100,
                        },
                        Quantity = 1,
                    }
                }
                });

                return stripeCheckoutSession;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create session.");
                throw new InvalidOperationException($"Failed to create session: {ex.Message}", ex);
            }
        }


        public async Task<Transaction> CreateTransaction(User user, decimal amount)
        {
            try
            {
                var tx = new Transaction
                {
                    UserId = user.Id,
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                    Type = amount > 0 ? TransactionType.Deposit : TransactionType.Withdrawal
                };
                _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();
                return tx;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create transaction.");
                throw new InvalidOperationException($"Failed to create transaction: {ex.Message}", ex);
            }
        }
    }
}