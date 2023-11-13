using Microsoft.AspNetCore.Identity;

namespace Akros.Security.UserManager.Infrastructure.Interfaces
{
    public interface IRolesManagerRepository
    {
        Task CreateRole(IdentityRole identityRole);
        Task<IdentityRole> GetByIdAsync(string roleid);
        Task<IdentityRole> GetByNameAsync(string role);
        Task<List<IdentityRole>> GetRolesAsync();
        Task UpdateRoleAsync(IdentityRole identityRole);
    }
}
