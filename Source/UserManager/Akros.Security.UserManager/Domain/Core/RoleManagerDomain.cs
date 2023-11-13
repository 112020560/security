using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Infrastructure.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Akros.Security.UserManager.Domain.Core
{
    public class RoleManagerDomain: IRoleManagerDomain
    {
        private readonly IRolesManagerRepository _rolesManagerRepository;
        private readonly IMapper _mapper;
        public RoleManagerDomain(IRolesManagerRepository rolesManagerRepository, IMapper mapper)
        {
            _rolesManagerRepository = rolesManagerRepository;
            _mapper = mapper;   
        }

        public async Task<List<RoleModel>> GetRolesAsync()
        {
            var roles = await _rolesManagerRepository.GetRolesAsync();
            if(roles != null && roles.Any())
            {
                return _mapper.Map<List<RoleModel>>(roles);
            }
            else
            {
                return new List<RoleModel>();
            }
        }

        public async Task CreateRoleAsync(RoleModel roleModel)
        {
            var IdentityRole = new IdentityRole(roleModel.Name);
            await _rolesManagerRepository.CreateRole(IdentityRole);
        }

        public async Task UpdateRoleAsync(RoleModel roleModel)
        {
            var IdentityRole = await _rolesManagerRepository.GetByIdAsync(roleModel.Id);
            IdentityRole.Name = roleModel.Name;
            await _rolesManagerRepository.UpdateRoleAsync(IdentityRole);
        }
    }
}
