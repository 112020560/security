using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Roles.Commands
{
    public partial class UpdateRoleCommand: IRequest<Response<bool>>
    {
        public readonly RoleModel roleModel;
        public UpdateRoleCommand(RoleModel roleModel)
        {
            this.roleModel = roleModel;
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Response<bool>>
    {
        private readonly IRoleManagerDomain _roleManagerDomain;
        public UpdateRoleCommandHandler(IRoleManagerDomain roleManagerDomain)
        {
            _roleManagerDomain = roleManagerDomain;
        }

        public async Task<Response<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            await _roleManagerDomain.UpdateRoleAsync(request.roleModel);
            return new Response<bool> { Success = true };
        }
    }
}
