using StripeBet.Models;
using StripeBet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StripeBet.Controllers
{
    public class BetController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<BetController> _logger;
        private readonly IBetService _betService;

        public BetController(IUserService userService, ILogger<BetController> logger, IBetService betService)
        {
            _userService = userService;
            _logger = logger;
            _betService = betService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Please log in to place a bet.";
                    return RedirectToAction("Login", "Account");
                }

                var user = _userService.GetUserByEmail(userEmail);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Login", "Account");
                }

                var model = new BetViewModel
                {
                    CurrentBalance = user.Balance
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Loading bet page failed");
                _logger.LogError(ex.Message);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Home", "Index");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Bet(BetViewModel model)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["ErrorMessage"] = "Please log in to place a bet.";
                    return RedirectToAction("Login", "Account");
                }

                if (ModelState.IsValid)
                {
                    if (!_betService.CanBet(userEmail, model.Amount))
                    {
                        ModelState.AddModelError("Amount", "Insufficient balance or invalid bet amount.");
                        var user = _userService.GetUserByEmail(userEmail);
                        model.CurrentBalance = user?.Balance ?? 0;
                        return View(model);
                    }

                    var random = new Random();
                    var randomNumber = random.Next(1, 101);

                    var resultColor = randomNumber % 2 == 0 ? BetColor.Red : BetColor.Black;
                    var won = model.BetColor == resultColor;

                    var success = await _betService.ProcessBet(userEmail, model.Amount, won);

                    if (success)
                    {
                        var updatedUser = _userService.GetUserByEmail(userEmail);
                        if (updatedUser != null)
                        {
                            HttpContext.Session.SetString("Balance", updatedUser.Balance.ToString());
                        }

                        var resultModel = new BetResultViewModel
                        {
                            BetAmount = model.Amount,
                            BetColor = model.BetColor,
                            RandomNumber = randomNumber,
                            ResultColor = resultColor,
                            Won = won,
                            Winnings = won ? model.Amount : 0,
                            NewBalance = updatedUser?.Balance ?? 0
                        };

                        await _betService.UpdateUserBets(userEmail, resultModel);

                        _logger.LogInformation($"User {userEmail} placed a bet of {model.Amount:C} on {model.BetColor}. " +
                                             $"Number: {randomNumber}, Result: {resultColor}, Won: {won}");

                        return View("Result", resultModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to process bet. Please try again.");
                        _logger.LogInformation("Failed to process bet");
                    }
                    var userForBalance = _userService.GetUserByEmail(userEmail);
                    model.CurrentBalance = userForBalance?.Balance ?? 0;
                    return View(model);
                }
                else {
                    _logger.LogError($"Model is not valid");
                    TempData["ErrorMessage"] = $"Model is not valid";
                    return RedirectToAction("Home", "Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Creating bet failed");
                _logger.LogError(ex.Message);
                TempData["ErrorMessage"] = $"Creating bet failed: {ex.Message}";
                return RedirectToAction("Home", "Index");
            }
        }

        [HttpGet]
        public IActionResult Result()
        {
            return RedirectToAction("Index");
        }
    }
}