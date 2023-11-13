using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Transversal.Common;
using MediatR;

namespace Akros.Security.UserManager.Application.Main.CQRS.Users.Command;

public partial class CreateUserCommand: IRequest<Response<bool>>
{
    public readonly CreateUserDto createUserDto;
    public CreateUserCommand(CreateUserDto createUserDto)
    {
        this.createUserDto = createUserDto;
    }
}

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Response<bool>>
{
    private readonly IUserManagerDomain _userManagerDomain;
    public CreateUserHandler(IUserManagerDomain userManagerDomain)
    {
        _userManagerDomain = userManagerDomain;
    }
    public async Task<Response<bool>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _userManagerDomain.CreateUserAsync(request.createUserDto);
        return new Response<bool>
        {
            Success = true,
            Data = response
        };
    }
}

