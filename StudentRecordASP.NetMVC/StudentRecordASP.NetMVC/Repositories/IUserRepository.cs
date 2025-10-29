using StudentRecordASP.NetMVC.Models;

namespace StudentRecordASP.NetMVC.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(User user, string passwordHash, int? studentId);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByStudentIdAsync(int studentId);
        Task UpdatePassword(int userId, string newPassword);
        Task UpdateUserWithStudentId(int userId, int studentId);
    }
}
