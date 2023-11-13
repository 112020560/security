using Akros.Authorizer.Application.Dto;
using Akros.Authorizer.Domain.Entities;
using AutoMapper;

namespace Akros.Authorizer.Transversal.Mapper.Mappers
{
    public class GeneralProfiler: Profile
    {
        public GeneralProfiler()
        {
            CreateMap<UserLoggedDto, UserModel>();
        }
    }
}
