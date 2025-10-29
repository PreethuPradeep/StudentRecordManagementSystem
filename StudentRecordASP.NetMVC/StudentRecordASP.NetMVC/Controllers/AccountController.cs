using Microsoft.AspNetCore.Mvc;
using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.Repositories;
using StudentRecordASP.NetMVC.Services;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Controllers
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Validate credentials
                var isValid = await _userService.ValidateUserCredentialsAsync(model.Email, model.Password);
                
                if (!isValid)
                {
                    // Log for debugging
                    var existingUser = await _userService.GetUserByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Login failed for {Email}. User exists but password doesn't match.", model.Email);
                    }
                    else
                    {
                        _logger.LogWarning("Login failed for {Email}. User not found.", model.Email);
                    }
                    
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                // Get user details
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(model);
                }

                // Store user info in session
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetString("IsDefaultPassword", user.IsDefaultPassword.ToString());

                // Redirect based on default password status
                if (user.IsDefaultPassword)
                {
                    return RedirectToAction("ChangePassword");
                }

                // Redirect based on role
                return RedirectToAction("Dashboard", GetDashboardController(user.UserRole));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            // Check if session exists
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return RedirectToAction("Login");
                }

                var userId = int.Parse(userIdString);

                // Verify old password
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(model);
                }

                var isOldPasswordValid = await _userService.VerifyPasswordAsync(model.OldPassword, user.PasswordHash);
                if (!isOldPasswordValid)
                {
                    ModelState.AddModelError("", "Current password is incorrect.");
                    return View(model);
                }

                // Update password
                await _userService.UpdatePasswordAsync(userId, model.NewPassword);

                // Update session
                HttpContext.Session.SetString("IsDefaultPassword", "False");

                // Redirect to dashboard
                var userRole = HttpContext.Session.GetString("UserRole");
                return RedirectToAction("Dashboard", GetDashboardController(userRole));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                ModelState.AddModelError("", "An error occurred while changing password. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string GetDashboardController(string userRole)
        {
            return userRole switch
            {
                "Admin" => "Admin",
                "Invigilator" => "Invigilator",
                "Student" => "Student",
                _ => "Admin"
            };
        }
    }
}
