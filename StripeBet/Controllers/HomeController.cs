using StripeBet.Models;
using StripeBet.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Forwarding;
using System.Diagnostics;
using System.Net;

namespace StripeBet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IStripeService _stripeService;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IStripeService stripeService)
        {
            _logger = logger;
            _userService = userService;
            _stripeService = stripeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> PayoutAsync(decimal amount, string paymentMethod)
        {
            try
            {
                var email = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(email))
                {
                    TempData["ErrorMessage"] = "Please log in to make a deposit.";
                    return RedirectToAction("Login", "Account");
                }

                var user = _userService.GetUserByEmail(email);
                if (amount > user.Balance)
                {
                    TempData["ErrorMessage"] = "Not enough balance!";
                    return RedirectToAction("Index");
                }

                var paymentIntent = await _stripeService.CreatePaymentIntentAsync(amount);

                var confirmedPaymentIntent = await _stripeService.ConfirmPaymentIntentAsync(paymentIntent, paymentMethod);

                _logger.LogInformation($"User {email} deposited {amount:C} via {paymentMethod}");
                TempData["SuccessMessage"] = $"Withdrawal of {amount:C} via {paymentMethod} processed successfully!";

                if (confirmedPaymentIntent.StripeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    TempData["ErrorMessage"] = $"Withdrawal of {amount:C} via {paymentMethod} failed!";
                }
                else
                {
                    _logger.LogInformation($"User {email} deposited {amount:C} via {paymentMethod}");
                    TempData["SuccessMessage"] = $"Withdrawal of {amount:C} via {paymentMethod} processed successfully!";
                    var updatedUser = await _userService.UpdateUserBalance(email, -amount);

                    if (updatedUser != null)
                    {
                        HttpContext.Session.SetString("Balance", updatedUser.Balance.ToString());
                    }
                }
                var tx = await _stripeService.CreateTransaction(user, -amount);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Redirect($"{Request.Scheme}://{Request.Host}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> BuyAsync(decimal amount, string paymentMethod)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Please log in to make a deposit.";
                    return RedirectToAction("Login", "Account");
                }
                var user = _userService.GetUserByEmail(userEmail);

                var defaultProduct = new DefaultProduct("1", "Default product", amount);
                var session = await _stripeService.BuyAsync(user, defaultProduct);

                if (session.StripeResponse.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"User {userEmail} deposited {amount:C} via {paymentMethod}");
                    TempData["SuccessMessage"] = $"Deposit of {amount:C} via {paymentMethod} processed successfully!";
                    var updatedUser = await _userService.UpdateUserBalance(userEmail, amount);

                    if (updatedUser != null)
                    {
                        HttpContext.Session.SetString("Balance", updatedUser.Balance.ToString());
                    }
                    var tx = await _stripeService.CreateTransaction(user, amount);
                    return Redirect(session.Url);
                }
                else
                {
                    TempData["ErrorMessage"] = $"Deposit of {amount:C} via {paymentMethod} failed!";
                    return Redirect(session.Url);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Redirect($"{Request.Scheme}://{Request.Host}");
            }
        }
    }
}
