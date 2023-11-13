using Akros.Security.UserManager.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Akros.Security.UserManager.Infrastructure.Repositories
{

    public class RolesManagerRepository : IRolesManagerRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesManagerRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityRole> GetByIdAsync(string roleid)
        {
            return await _roleManager.FindByIdAsync(roleid);
        }

        public async Task<IdentityRole> GetByNameAsync(string role)
        {
            return await _roleManager.FindByNameAsync(role);
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task CreateRole(IdentityRole identityRole)
        {
            await _roleManager.CreateAsync(identityRole);
        }

        public async Task UpdateRoleAsync(IdentityRole identityRole)
        {
            await _roleManager.UpdateAsync(identityRole);
        }
    }
}
