using Microsoft.Data.SqlClient;
using StudentRecordASP.NetMVC.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudentRecordASP.NetMVC.Repositories
{
    public class UserRepositoryImplementation : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepositoryImplementation(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string not found");
        }

        public async Task<int> CreateUserAsync(User user, string passwordHash, int? studentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                                 INSERT INTO TblUser (Email, PasswordHash, UserRole, IsDefaultPassword, StudentId)
                                 VALUES (@Email, @PasswordHash, @UserRole, @IsDefaultPassword, @StudentId);
                                 SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@UserRole", user.UserRole);
            command.Parameters.AddWithValue("@IsDefaultPassword", user.IsDefaultPassword);
            command.Parameters.AddWithValue("@StudentId", studentId.HasValue ? (object)studentId.Value : DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                                SELECT UserId, Email, PasswordHash, UserRole, IsDefaultPassword, StudentId
                                FROM TblUser
                                WHERE Email = @Email";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    UserRole = reader.GetString(3),
                    IsDefaultPassword = reader.GetBoolean(4),
                    StudentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                };
            }

            return null;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                                 SELECT UserId, Email, PasswordHash, UserRole, IsDefaultPassword, StudentId
                                 FROM TblUser
                                 WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    UserRole = reader.GetString(3),
                    IsDefaultPassword = reader.GetBoolean(4),
                    StudentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                };
            }

            return null;
        }

        public async Task<User> GetUserByStudentIdAsync(int studentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                                 SELECT UserId, Email, PasswordHash, UserRole, IsDefaultPassword, StudentId
                                 FROM TblUser
                                 WHERE StudentId = @StudentId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StudentId", studentId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    UserRole = reader.GetString(3),
                    IsDefaultPassword = reader.GetBoolean(4),
                    StudentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                };
            }

            return null;
        }

        public async Task UpdatePassword(int userId, string newPassword)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                UPDATE TblUser 
                SET PasswordHash = @PasswordHash, IsDefaultPassword = 0
                WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@PasswordHash", newPassword);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateUserWithStudentId(int userId, int studentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                UPDATE TblUser 
                SET StudentId = @StudentId
                WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@StudentId", studentId);

            await command.ExecuteNonQueryAsync();
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            // Convert to uppercase hex string to match SQL Server's CONVERT(NVARCHAR, BINARY, 2)
            // Style 2 produces uppercase hexadecimal without dashes
            var hexString = BitConverter.ToString(hashedBytes).Replace("-", "");
            return hexString;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            var hashedInput = HashPassword(password);
            
            // DEBUG: Log hash values for comparison
            // You can see these in Debug Output window or remove this after debugging
            System.Diagnostics.Debug.WriteLine($"=== Password Verification Debug ===");
            System.Diagnostics.Debug.WriteLine($"Password: {password}");
            System.Diagnostics.Debug.WriteLine($"Generated Hash: {hashedInput}");
            System.Diagnostics.Debug.WriteLine($"Stored Hash: {hashedPassword}");
            System.Diagnostics.Debug.WriteLine($"Hash Length Match: {hashedInput.Length == hashedPassword.Length}");
            System.Diagnostics.Debug.WriteLine($"Hash Match: {hashedInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase)}");
            System.Diagnostics.Debug.WriteLine($"====================================");
            
            // Case-insensitive comparison to handle any case differences
            return hashedInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}