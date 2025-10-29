using Microsoft.AspNetCore.Mvc;
using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.Services;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IUserService userService, IStudentService studentService, ILogger<StudentController> logger)
        {
            _userService = userService;
            _studentService = studentService;
            _logger = logger;
        }

        // GET: Student/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Check if user is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Student")
            {
                return RedirectToAction("Dashboard", userRole);
            }

            try
            {
                // Get user details
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userId = int.Parse(userIdString);
                
                // Get student ID from user
                var studentId = await _userService.GetStudentIdByUserIdAsync(userId);

                if (!studentId.HasValue)
                {
                    TempData["ErrorMessage"] = "Student information not found.";
                    return View();
                }

                // Get student details using the student ID directly
                var allStudents = await _studentService.GetAllActiveStudentsAsync();
                var student = allStudents.FirstOrDefault(s => s.Id == studentId.Value);

                if (student == null)
                {
                    TempData["ErrorMessage"] = "Student record not found.";
                    return View();
                }

                return View(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading student dashboard");
                TempData["ErrorMessage"] = "An error occurred while loading your information.";
                return View();
            }
        }
    }
}
