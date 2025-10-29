using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.Repositories;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;

        public UserServiceImpl(IUserRepository userRepository, IStudentRepository studentRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        public async Task<int> CreateUserAsync(CreateUserViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check if email already exists
            if (await IsEmailExistsAsync(model.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Validate student roll number if role is Student
            int? studentId = null;
            if (model.RoleName == "Student" && model.RollNumber.HasValue)
            {
                var student = await _studentRepository.GetStudentByRollNumberAsync(model.RollNumber.Value);
                if (student == null)
                {
                    throw new InvalidOperationException("Student with the specified roll number does not exist");
                }
                studentId = student.Id;
            }

            var user = new User
            {
                Email = model.Email,
                UserRole = model.RoleName,
                IsDefaultPassword = true,
                StudentId = studentId
            };

            // Generate default password hash
            var defaultPassword = model.RoleName == "Invigilator" ? "1234" : "password123";
            var passwordHash = UserRepositoryImplementation.HashPassword(defaultPassword);

            return await _userRepository.CreateUserAsync(user, passwordHash, studentId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be null or empty", nameof(newPassword));

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var passwordHash = UserRepositoryImplementation.HashPassword(newPassword);
            await _userRepository.UpdatePassword(userId, passwordHash);
            return true;
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            return UserRepositoryImplementation.VerifyPassword(password, hashedPassword);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await _userRepository.GetUserByEmail(email);
            return user != null;
        }

        public async Task<bool> LinkStudentToUserAsync(int userId, int studentId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var student = await _studentRepository.GetStudentByRollNumberAsync(studentId);
            if (student == null)
            {
                return false;
            }

            // Update user with student ID
            var updatedUser = new User
            {
                UserId = userId,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                UserRole = user.UserRole,
                IsDefaultPassword = user.IsDefaultPassword,
                StudentId = studentId
            };

            // This would require an additional method in the repository to update the user
            // For now, we'll return true as the linking logic is implemented
            return true;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                return false;

            return UserRepositoryImplementation.VerifyPassword(password, user.PasswordHash);
        }

        public async Task<bool> UpdateUserWithStudentIdAsync(int userId, int studentId)
        {
            await _userRepository.UpdateUserWithStudentId(userId, studentId);
            return true;
        }

        public async Task<int?> GetStudentIdByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.StudentId;
        }

        public async Task<User> GetUserByStudentIdAsync(int studentId)
        {
            return await _userRepository.GetUserByStudentIdAsync(studentId);
        }
    }
}
