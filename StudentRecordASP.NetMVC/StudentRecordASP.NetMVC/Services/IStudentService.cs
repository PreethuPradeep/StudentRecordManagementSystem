using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Services
{
    public interface IStudentService
    {
        Task<int> CreateStudentAsync(CreateStudentViewModel model);
        Task<Student> GetStudentByRollNumberAsync(int rollNumber);
        Task<Student> GetStudentByNameAsync(string name);
        Task<IEnumerable<Student>> GetAllActiveStudentsAsync();
        Task<bool> UpdateStudentMarksAsync(EditStudentViewModel model);
        Task<bool> DisableStudentAsync(int rollNumber);
        Task<LinkStudentToUser> GetStudentLinkDetailsAsync(int rollNumber);
        Task<bool> IsRollNumberExistsAsync(int rollNumber);
        Task<bool> IsStudentNameExistsAsync(string name);
    }
}
