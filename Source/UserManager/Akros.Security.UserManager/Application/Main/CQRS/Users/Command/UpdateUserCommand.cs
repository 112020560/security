using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Command
{
    public partial class UpdateUserCommand: IRequest<Response<bool>>
    {
        public readonly CreateUserDto createUserDto;
        public UpdateUserCommand(CreateUserDto createUserDto)
        {
            this.createUserDto = createUserDto;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<bool>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public UpdateUserCommandHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }

        public async Task<Response<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            await _userManagerDomain.UpdateUserAsync(request.createUserDto);
            return new Response<bool>
            {
                Success = true,
            };
        }
    }
}
