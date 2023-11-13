using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Command
{
    public class ChangePasswordCommand : IRequest<Response<bool>>
    {
        public readonly ChangePasswordDto ChangePasswordDto;
        public ChangePasswordCommand(ChangePasswordDto ChangePasswordDto)
        {
            this.ChangePasswordDto = ChangePasswordDto;
        }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Response<bool>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public ChangePasswordCommandHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }
        public async Task<Response<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await _userManagerDomain.ChangePasswordAsync(request.ChangePasswordDto);
            return new Response<bool>
            {
                Success = true,
            };
        }
    }
}
