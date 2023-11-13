using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Command
{
    public partial class CreateManyUserCommand : IRequest<Response<List<bool>>>
    {
        public readonly List<CreateUserDto> createUserDto;
        public CreateManyUserCommand(List<CreateUserDto> createUserDto)
        {
            this.createUserDto = createUserDto;
        }
    }

    public class CreateManyUserHandler : IRequestHandler<CreateManyUserCommand, Response<List<bool>>>
    {
        private readonly IUserManagerDomain _userManagerDomain;
        public CreateManyUserHandler(IUserManagerDomain userManagerDomain)
        {
            _userManagerDomain = userManagerDomain;
        }
        public async Task<Response<List<bool>>> Handle(CreateManyUserCommand request, CancellationToken cancellationToken)
        {
            List<bool> responses = new List<bool>();
            foreach (var dto in request.createUserDto)
            {
                responses.Add(await _userManagerDomain.CreateUserAsync(dto));
            }

            return new Response<List<bool>>
            {
                Success = true,
                Data = responses,
                Message = "users created successfully"
            };
        }
    }
}
