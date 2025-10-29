using Microsoft.Data.SqlClient;
using StudentRecordASP.NetMVC.Models;

namespace StudentRecordASP.NetMVC.Repositories
{
    public class RoleRepositoryImplementation : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepositoryImplementation(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string not found");
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            var roles = new List<Role>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT RoleId, RoleName, IsActive
                FROM TblRole
                WHERE IsActive = 1
                ORDER BY RoleName";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(new Role
                {
                    RoleId = reader.GetInt32(0),
                    RoleName = reader.GetString(1),
                    IsActive = reader.GetBoolean(2)
                });
            }

            return roles;
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT RoleId, RoleName, IsActive
                FROM TblRole
                WHERE RoleName = @RoleName AND IsActive = 1";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Role
                {
                    RoleId = reader.GetInt32(0),
                    RoleName = reader.GetString(1),
                    IsActive = reader.GetBoolean(2)
                };
            }

            return null;
        }
    }
}
