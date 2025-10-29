using StudentRecordASP.NetMVC.Models;

namespace StudentRecordASP.NetMVC.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}
