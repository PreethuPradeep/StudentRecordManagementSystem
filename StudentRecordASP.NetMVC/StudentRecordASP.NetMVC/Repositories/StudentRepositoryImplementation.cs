using Microsoft.Data.SqlClient;
using StudentRecordASP.NetMVC.Models;
using StudentRecordASP.NetMVC.ViewModel;
using System.Data;

namespace StudentRecordASP.NetMVC.Repositories
{
    public class StudentRepositoryImplementation : IStudentRepository
    {
        private readonly string _connectionString;

        public StudentRepositoryImplementation(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string not found");
        }

        public async Task<int> CreateStudentAsync(Student student)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO TblStudent (RollNumber, Name, Maths, Physics, Chemistry, English, Programming, IsActive)
                VALUES (@RollNumber, @Name, @Maths, @Physics, @Chemistry, @English, @Programming, @IsActive);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollNumber", student.RolLNumber);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Maths", student.Maths);
            command.Parameters.AddWithValue("@Physics", student.Physics);
            command.Parameters.AddWithValue("@Chemistry", student.Chemistry);
            command.Parameters.AddWithValue("@English", student.English);
            command.Parameters.AddWithValue("@Programming", student.Programming);
            command.Parameters.AddWithValue("@IsActive", student.IsActive);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<LinkStudentToUser> GetStudentLinkDetailsAsync(int RollNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT s.Id as StudentId, s.Name, u.UserId
                FROM TblStudent s
                LEFT JOIN TblUser u ON s.Id = u.StudentId
                WHERE s.RollNumber = @RollNumber AND s.IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollNumber", RollNumber);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LinkStudentToUser
                {
                    StudentId = reader.GetInt32("StudentId"),
                    Name = reader.GetString("Name"),
                    UserId = reader.IsDBNull("UserId") ? 0 : reader.GetInt32("UserId")
                };
            }

            return null;
        }

        public async Task<Student> GetStudentByRollNumberAsync(int RollNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT Id, RollNumber, Name, Maths, Physics, Chemistry, English, Programming, IsActive
                FROM TblStudent
                WHERE RollNumber = @RollNumber AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollNumber", RollNumber);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Student
                {
                    Id = reader.GetInt32("Id"),
                    RolLNumber = reader.GetInt32("RollNumber"),
                    Name = reader.GetString("Name"),
                    Maths = reader.GetInt32("Maths"),
                    Physics = reader.GetInt32("Physics"),
                    Chemistry = reader.GetInt32("Chemistry"),
                    English = reader.GetInt32("English"),
                    Programming = reader.GetInt32("Programming"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }

            return null;
        }

        public async Task<Student> GetStudentByNameAsync(string Name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT Id, RollNumber, Name, Maths, Physics, Chemistry, English, Programming, IsActive
                FROM TblStudent
                WHERE Name = @Name AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", Name);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Student
                {
                    Id = reader.GetInt32("Id"),
                    RolLNumber = reader.GetInt32("RollNumber"),
                    Name = reader.GetString("Name"),
                    Maths = reader.GetInt32("Maths"),
                    Physics = reader.GetInt32("Physics"),
                    Chemistry = reader.GetInt32("Chemistry"),
                    English = reader.GetInt32("English"),
                    Programming = reader.GetInt32("Programming"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }

            return null;
        }

        public async Task<IEnumerable<Student>> GetAllActiveStudentsAsync()
        {
            var students = new List<Student>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT Id, RollNumber, Name, Maths, Physics, Chemistry, English, Programming, IsActive
                FROM TblStudent
                WHERE IsActive = 1
                ORDER BY RollNumber";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                students.Add(new Student
                {
                    Id = reader.GetInt32("Id"),
                    RolLNumber = reader.GetInt32("RollNumber"),
                    Name = reader.GetString("Name"),
                    Maths = reader.GetInt32("Maths"),
                    Physics = reader.GetInt32("Physics"),
                    Chemistry = reader.GetInt32("Chemistry"),
                    English = reader.GetInt32("English"),
                    Programming = reader.GetInt32("Programming"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }

            return students;
        }

        public async Task UpdateMarksAsync(Student student)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                UPDATE TblStudent 
                SET Maths = @Maths, Physics = @Physics, Chemistry = @Chemistry, English = @English, Programming = @Programming
                WHERE RollNumber = @RollNumber AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollNumber", student.RolLNumber);
            command.Parameters.AddWithValue("@Maths", student.Maths);
            command.Parameters.AddWithValue("@Physics", student.Physics);
            command.Parameters.AddWithValue("@Chemistry", student.Chemistry);
            command.Parameters.AddWithValue("@English", student.English);
            command.Parameters.AddWithValue("@Programming", student.Programming);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DisableStudentAsync(int rollNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                UPDATE TblStudent 
                SET IsActive = 0
                WHERE RollNumber = @RollNumber";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollNumber", rollNumber);

            await command.ExecuteNonQueryAsync();
        }
    }
}
