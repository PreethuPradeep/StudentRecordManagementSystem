using Microsoft.AspNetCore.Mvc;
using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.Services;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Controllers
{
    public class InvigilatorController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly ILogger<InvigilatorController> _logger;

        public InvigilatorController(IStudentService studentService, IUserService userService, ILogger<InvigilatorController> logger)
        {
            _studentService = studentService;
            _userService = userService;
            _logger = logger;
        }

        // GET: Invigilator/Dashboard
        public async Task<IActionResult> Dashboard(string searchString)
        {
            // Check if user is authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Invigilator")
            {
                return RedirectToAction("Dashboard", userRole);
            }

            IEnumerable<Student> students;

            if (!string.IsNullOrEmpty(searchString))
            {
                // Search by name or roll number
                var searchResults = new List<Student>();

                // Try to parse as roll number
                if (int.TryParse(searchString, out int rollNumber))
                {
                    var student = await _studentService.GetStudentByRollNumberAsync(rollNumber);
                    if (student != null)
                    {
                        searchResults.Add(student);
                    }
                }

                // Search by name (partial match)
                var allStudents = await _studentService.GetAllActiveStudentsAsync();
                searchResults.AddRange(allStudents.Where(s => 
                    s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)));

                students = searchResults.Distinct();
            }
            else
            {
                students = await _studentService.GetAllActiveStudentsAsync();
            }

            ViewBag.SearchString = searchString;
            return View(students);
        }

        // GET: Invigilator/CreateStudent
        public IActionResult CreateStudent()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Invigilator/CreateStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var studentId = await _studentService.CreateStudentAsync(model);

                if (studentId > 0)
                {
                    // Get the created student to retrieve roll number
                    var allStudents = await _studentService.GetAllActiveStudentsAsync();
                    var createdStudent = allStudents.FirstOrDefault(s => s.Id == studentId);
                    
                    if (createdStudent != null)
                    {
                        // Get email from form (if manually edited) or generate it
                        string finalEmail;
                        var formEmail = Request.Form["FinalEmail"].ToString();
                        if (!string.IsNullOrWhiteSpace(formEmail))
                        {
                            finalEmail = formEmail;
                        }
                        else
                        {
                            // Generate email in firstnamelastname@gmail.com format
                            var username = GenerateUsernameFromName(model.Name);
                            
                            // Ensure email is unique
                            finalEmail = await EnsureUniqueEmail(username);
                        }
                        
                        // Create user account automatically
                        var createUserModel = new CreateUserViewModel
                        {
                            Email = finalEmail,
                            RoleName = "Student",
                            RollNumber = createdStudent.RolLNumber
                        };

                        try
                        {
                            var userId = await _userService.CreateUserAsync(createUserModel);
                            if (userId > 0)
                            {
                                TempData["SuccessMessage"] = $"Student created successfully!<br/><strong>Login Email:</strong> {finalEmail}<br/><strong>Password:</strong> password123";
                            }
                            else
                            {
                                TempData["SuccessMessage"] = "Student created successfully, but user account creation failed.";
                            }
                        }
                        catch (Exception userEx)
                        {
                            _logger.LogWarning(userEx, "Failed to create user account for student {StudentId}", studentId);
                            TempData["SuccessMessage"] = $"Student created successfully, but user account creation failed: {userEx.Message}";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Student created successfully.";
                    }

                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create student.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: Invigilator/EditStudent/5
        public async Task<IActionResult> EditStudent(int rollNumber)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            var student = await _studentService.GetStudentByRollNumberAsync(rollNumber);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToAction("Dashboard");
            }

            var viewModel = new EditStudentViewModel
            {
                RollNumber = student.RolLNumber,
                Name = student.Name,
                Maths = student.Maths,
                Physics = student.Physics,
                Chemistry = student.Chemistry,
                English = student.English,
                Programming = student.Programming
            };

            return View(viewModel);
        }

        // POST: Invigilator/EditStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(EditStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _studentService.UpdateStudentMarksAsync(model);

                if (result)
                {
                    TempData["SuccessMessage"] = "Student marks updated successfully.";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update student marks.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student");
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // POST: Invigilator/DeleteStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int rollNumber)
        {
            try
            {
                var result = await _studentService.DisableStudentAsync(rollNumber);

                if (result)
                {
                    TempData["SuccessMessage"] = "Student deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete student.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student");
                TempData["ErrorMessage"] = "An error occurred while deleting the student.";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: Invigilator/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int rollNumber)
        {
            var student = await _studentService.GetStudentByRollNumberAsync(rollNumber);
            
            if (student == null)
            {
                return NotFound();
            }

            // Get user login credentials if linked
            string? loginEmail = null;
            bool isDefaultPassword = false;
            
            if (student.Id > 0)
            {
                var userAccount = await _userService.GetUserByStudentIdAsync(student.Id);
                if (userAccount != null)
                {
                    loginEmail = userAccount.Email;
                    isDefaultPassword = userAccount.IsDefaultPassword;
                }
            }

            ViewBag.LoginEmail = loginEmail;
            ViewBag.IsDefaultPassword = isDefaultPassword;
            ViewBag.DefaultPassword = "password123"; // Default password for students

            return PartialView("_StudentDetails", student);
        }

        // GET: Invigilator/CheckEmailExists
        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(false);
            }

            var exists = await _userService.IsEmailExistsAsync(email);
            return Json(exists);
        }

        /// <summary>
        /// Generates email/username from student name in firstnamelastname@gmail.com format
        /// </summary>
        private string GenerateUsernameFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            // Split name into parts
            var nameParts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (nameParts.Length == 0)
                throw new ArgumentException("Name must contain at least one word", nameof(name));

            string usernamePart;

            // If only one part, use it as username
            if (nameParts.Length == 1)
            {
                usernamePart = nameParts[0];
            }
            else
            {
                // Combine first name and last name (all parts except first are last name)
                var firstName = nameParts[0];
                var lastName = string.Join("", nameParts.Skip(1)); // Join remaining parts without space
                usernamePart = firstName + lastName;
            }

            // Convert to lowercase and append @gmail.com
            return usernamePart.ToLowerInvariant() + "@gmail.com";
        }

        /// <summary>
        /// Ensures email is unique by appending a number if needed
        /// </summary>
        private async Task<string> EnsureUniqueEmail(string baseEmail)
        {
            var email = baseEmail;
            int counter = 1;

            // Extract the local part (before @) and domain part (after @)
            var parts = baseEmail.Split('@');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid email format", nameof(baseEmail));

            var localPart = parts[0];
            var domainPart = parts[1];

            while (await _userService.IsEmailExistsAsync(email))
            {
                email = $"{localPart}{counter}@{domainPart}";
                counter++;
                
                // Prevent infinite loop (very unlikely scenario)
                if (counter > 1000)
                    throw new InvalidOperationException("Unable to generate unique email");
            }

            return email;
        }
    }
}
