using Microsoft.AspNetCore.Mvc;
using StudentRecordASP.NetMVC.Repositories;
using StudentRecordASP.NetMVC.Services;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserService userService, IRoleRepository roleRepository, ILogger<AdminController> logger)
        {
            _userService = userService;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Check if user is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Dashboard", userRole);
            }

            return View();
        }

        // GET: Admin/CreateUser
        public async Task<IActionResult> CreateUser()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Load roles from database
            var roles = await _roleRepository.GetAllRolesAsync();
            ViewBag.Roles = roles;

            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Load roles in case of validation error
                var roles = await _roleRepository.GetAllRolesAsync();
                ViewBag.Roles = roles;

                // Create the user
                var userId = await _userService.CreateUserAsync(model);

                if (userId > 0)
                {
                    var defaultPassword = model.RoleName == "Invigilator" ? "1234" : "password123";
                    TempData["SuccessMessage"] = $"User created successfully with default password: {defaultPassword}";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create user.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                
                // Load roles in case of error
                var roles = await _roleRepository.GetAllRolesAsync();
                ViewBag.Roles = roles;
                
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}
