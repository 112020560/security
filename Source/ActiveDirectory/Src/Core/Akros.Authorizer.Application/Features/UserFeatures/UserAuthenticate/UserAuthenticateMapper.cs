using Akros.Authorizer.Domain.Entities.Persistence;
using AutoMapper;

namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public sealed class UserAuthenticateMapper: Profile
{
    public UserAuthenticateMapper()
    {
        CreateMap<UserAuthenticateRequest, User>()
            .ForMember(dest => dest.UserName, source => source.MapFrom(src => src.User));

        CreateMap<UserAuthenticateRequestV3, User>();
    }
}
