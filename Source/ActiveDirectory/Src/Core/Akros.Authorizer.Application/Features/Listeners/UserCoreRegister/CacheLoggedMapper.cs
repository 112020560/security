using Akros.Authorizer.Domain.Entities.Mongo;
using AutoMapper;

namespace Akros.Authorizer.Application.Features.Listeners.UserCoreRegister
{
    public class CacheLoggedMapper: Profile
    {
        public CacheLoggedMapper()
        {
            CreateMap<CacheLoggedContract, LoggedCache>();
        }
    }
}
