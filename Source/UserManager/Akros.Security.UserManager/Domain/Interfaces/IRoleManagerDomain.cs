using Akros.Security.UserManager.Domain.Models;

namespace Akros.Security.UserManager.Domain.Interfaces
{
    public interface IRoleManagerDomain
    {
        Task<List<RoleModel>> GetRolesAsync();
        Task CreateRoleAsync(RoleModel roleModel);
        Task UpdateRoleAsync(RoleModel roleModel);
    }
}
