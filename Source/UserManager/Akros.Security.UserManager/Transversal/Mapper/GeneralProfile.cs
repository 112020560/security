using Akros.Security.UserManager.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Akros.Security.UserManager.Transversal.Mapper
{
    public class GeneralProfile: Profile
    {
        public GeneralProfile()
        {
            CreateMap<IdentityRole, RoleModel>().ReverseMap();
            CreateMap<ApplicationUser, UserModel>().ReverseMap();
        }
    }
}
