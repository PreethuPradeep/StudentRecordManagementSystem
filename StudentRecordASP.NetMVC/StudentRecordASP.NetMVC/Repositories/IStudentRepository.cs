using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Repositories
{
    public interface IStudentRepository
    {
        Task<int> CreateStudentAsync(Student student);
        Task<LinkStudentToUser> GetStudentLinkDetailsAsync(int RollNumber);
        Task<Student> GetStudentByRollNumberAsync(int RollNumber);
        Task<Student> GetStudentByNameAsync(string Name);
        Task<IEnumerable<Student>> GetAllActiveStudentsAsync();
        Task UpdateMarksAsync(Student student);
        Task DisableStudentAsync(int rollNumber);
    }
}
