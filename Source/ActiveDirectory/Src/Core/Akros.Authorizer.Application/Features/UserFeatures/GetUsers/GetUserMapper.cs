using Akros.Authorizer.Domain.Entities.Persistence;
using AutoMapper;

namespace Akros.Authorizer.Application.Features.UserFeatures.GetUsers;

public sealed class GetUserMapper: Profile
{
    public GetUserMapper()
    {
        CreateMap<User, GetUserResponse>();
    }
}
