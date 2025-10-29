using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Services
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserViewModel model);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> LinkStudentToUserAsync(int userId, int studentId);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
        Task<bool> UpdateUserWithStudentIdAsync(int userId, int studentId);
        Task<int?> GetStudentIdByUserIdAsync(int userId);
        Task<User> GetUserByStudentIdAsync(int studentId);
    }
}
