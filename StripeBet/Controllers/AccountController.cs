using StripeBet.Models;
using StripeBet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace StripeBet.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.ValidateUser(model.Email, model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Balance", user.Balance.ToString());

                    TempData["SuccessMessage"] = $"Welcome, {user.Username}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_userService.EmailExists(model.Email))
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(model);
                    }

                    var user = await _userService.CreateUser(model.Username, model.Email, model.Password);
                    if (user != null)
                    {
                        TempData["SuccessMessage"] = $"Account created successfully! Welcome, {user.Username}!";

                        // Automatically log in the user after registration
                        HttpContext.Session.SetString("Username", user.Username);
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("Balance", user.Balance.ToString());
                        HttpContext.Session.SetString("Email", user.Email);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to create account. Please try again.");
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["ErrorMessage"] = "Failed to register.";
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult GetUserInfo()
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

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData["ErrorMessage"] = "Failed to get uesr data!";
                return RedirectToAction("Home","Index");
            }

        }
    }
}

