using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Roles.Commands
{
    public class CreteRoleCommand: IRequest<Response<bool>>
    {
        public readonly RoleModel roleModel;
        public CreteRoleCommand(RoleModel roleModel)
        {
            this.roleModel = roleModel;
        }
    }

    public class CreteRoleHandler : IRequestHandler<CreteRoleCommand, Response<bool>>
    {
        private readonly IRoleManagerDomain _roleManagerDomain;
        public CreteRoleHandler(IRoleManagerDomain roleManagerDomain)
        {
            _roleManagerDomain = roleManagerDomain;
        }
        public async Task<Response<bool>> Handle(CreteRoleCommand request, CancellationToken cancellationToken)
        {
            await _roleManagerDomain.CreateRoleAsync(request.roleModel);
            return new Response<bool>
            {
                Success = true,
            };
        }
    }
}
