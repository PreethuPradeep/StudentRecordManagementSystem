using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.Repositories;
using StudentRecordASP.NetMVC.ViewModel;

namespace StudentRecordASP.NetMVC.Services
{
    public class StudentServiceImpl : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentServiceImpl(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        public async Task<int> CreateStudentAsync(CreateStudentViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Get next roll number
            var nextRollNumber = await GetNextRollNumber();

            // Check if roll number already exists
            if (await IsRollNumberExistsAsync(nextRollNumber))
            {
                throw new InvalidOperationException("Roll number already exists");
            }

            // Check if student name already exists
            if (await IsStudentNameExistsAsync(model.Name))
            {
                throw new InvalidOperationException("Student with this name already exists");
            }

            var student = new Student
            {
                RolLNumber = nextRollNumber,
                Name = model.Name,
                Maths = model.Maths,
                Physics = model.Physics,
                Chemistry = model.Chemistry,
                English = model.English,
                Programming = model.Programming,
                IsActive = true
            };

            return await _studentRepository.CreateStudentAsync(student);
        }

        public async Task<Student> GetStudentByRollNumberAsync(int rollNumber)
        {
            return await _studentRepository.GetStudentByRollNumberAsync(rollNumber);
        }

        public async Task<Student> GetStudentByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            return await _studentRepository.GetStudentByNameAsync(name);
        }

        public async Task<IEnumerable<Student>> GetAllActiveStudentsAsync()
        {
            return await _studentRepository.GetAllActiveStudentsAsync();
        }

        public async Task<bool> UpdateStudentMarksAsync(EditStudentViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingStudent = await _studentRepository.GetStudentByRollNumberAsync(model.RollNumber);
            if (existingStudent == null)
            {
                return false;
            }

            var student = new Student
            {
                RolLNumber = model.RollNumber,
                Name = model.Name,
                Maths = model.Maths,
                Physics = model.Physics,
                Chemistry = model.Chemistry,
                English = model.English,
                Programming = model.Programming,
                IsActive = true
            };

            await _studentRepository.UpdateMarksAsync(student);
            return true;
        }

        public async Task<bool> DisableStudentAsync(int rollNumber)
        {
            var existingStudent = await _studentRepository.GetStudentByRollNumberAsync(rollNumber);
            if (existingStudent == null)
            {
                return false;
            }

            await _studentRepository.DisableStudentAsync(rollNumber);
            return true;
        }

        public async Task<LinkStudentToUser> GetStudentLinkDetailsAsync(int rollNumber)
        {
            return await _studentRepository.GetStudentLinkDetailsAsync(rollNumber);
        }

        public async Task<bool> IsRollNumberExistsAsync(int rollNumber)
        {
            var student = await _studentRepository.GetStudentByRollNumberAsync(rollNumber);
            return student != null;
        }

        public async Task<bool> IsStudentNameExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var student = await _studentRepository.GetStudentByNameAsync(name);
            return student != null;
        }

        private async Task<int> GetNextRollNumber()
        {
            var students = await _studentRepository.GetAllActiveStudentsAsync();
            if (!students.Any())
            {
                return 1;
            }

            return students.Max(s => s.RolLNumber) + 1;
        }
    }
}
