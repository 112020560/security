using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Roles.Queries
{
    public class RolesGetAllQuery: IRequest<Response<List<RoleModel>>>
    {
    }

    public class RolesGetAllQueryHandler : IRequestHandler<RolesGetAllQuery, Response<List<RoleModel>>>
    {
        private readonly IRoleManagerDomain _roleManagerDomain;
        public RolesGetAllQueryHandler(IRoleManagerDomain roleManagerDomain)
        {
            _roleManagerDomain = roleManagerDomain;
        }
        public async Task<Response<List<RoleModel>>> Handle(RolesGetAllQuery request, CancellationToken cancellationToken)
        {
            return new Response<List<RoleModel>>
            {
                Success = true,
                Data = await _roleManagerDomain.GetRolesAsync()
            };
        }
    }
}
