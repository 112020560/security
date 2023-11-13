using Akros.Security.UserManager.Application.Dtos;
using Akros.Security.UserManager.Domain.Interfaces;
using Akros.Security.UserManager.Domain.Models;
using Akros.Security.UserManager.Infrastructure.Interfaces;
using AutoMapper;
using System.Security.Claims;

namespace Akros.Security.UserManager.Domain.Core
{
    public class UserManagerDomain : IUserManagerDomain
    {
        private readonly IUserManagerRepository _userManagerRepository;
        private readonly IMapper _mapper;
        public UserManagerDomain(IUserManagerRepository userManagerRepository, IMapper mapper)
        {
            _userManagerRepository = userManagerRepository;
            _mapper = mapper;
        }
        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = await _userManagerRepository.GetUserByUserNameAsync(createUserDto.UserName);
            if (user != null) throw new Exception($"The username {createUserDto.UserName} already exists");

            user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                ActiveDirectory = createUserDto.IsActiveDirectory,
                Country = createUserDto.Country,
                IsActive = createUserDto.IsActive,
                NormalizedEmail = createUserDto.Email.ToUpper(),
                NormalizedUserName = createUserDto.UserName.ToUpper(),
            };

            var result = await _userManagerRepository.CreateUserAsync(user, createUserDto.Password);
            if (result.Succeeded)
            {
                if (createUserDto.Roles != null && createUserDto.Roles.Length > 0)
                    await _userManagerRepository.CreateUserRoleAsync(user, createUserDto.Roles);

                Dictionary<string, string>? claims = new()
                {
                    {"name", createUserDto.FullName },
                    {"email", createUserDto.Email},
                    {"companyid", createUserDto.Company},
                    {"username", createUserDto.UserName},
                    {"role", createUserDto.Roles[0]}
                };

                if (!string.IsNullOrEmpty(createUserDto.Supervisor))
                {
                    claims.Add("supervisor", createUserDto.Supervisor);
                }

                if (claims.Count > 1)
                    await _userManagerRepository.CreateUserListClaimAsync(user, claims);

                return true;
            }
            return false;
        }

        public async Task UpdateUserAsync(CreateUserDto createUserDto)
        {
                var user = await _userManagerRepository.GetUserByUserNameAsync(createUserDto.UserName) ?? throw new Exception($"The username {createUserDto.UserName} does not exist");
            var update = false;
            var claims = await _userManagerRepository.GetUserCliamsAsync(user);

            if(!string.IsNullOrEmpty(createUserDto.FullName))
            {
                if (claims != null && claims.Any(x => x.Type.Equals("name")))
                {
                    var claim = claims.Find(x => x.Type.Equals("name"));
                    await _userManagerRepository.RemoveUserClaimAsync(user, claim);
                }
                var newClaim = new Claim("name", createUserDto.FullName);
                await _userManagerRepository.CreateUserClaimAsync(user, newClaim);
            }

            if (!string.IsNullOrEmpty(createUserDto.Company))
            {
                
                if (claims != null && claims.Any(x => x.Type.Equals("companyid")))
                {
                    var claim = claims.Find(x => x.Type.Equals("companyid"));
                    await _userManagerRepository.RemoveUserClaimAsync(user, claim);
                }
                var newClaim = new Claim("companyid", createUserDto.Company);
                await _userManagerRepository.CreateUserClaimAsync(user, newClaim);
            }

            if (!string.IsNullOrEmpty(createUserDto.Supervisor))
            {
                if (claims != null && claims.Any(x => x.Type.Equals("supervisor")))
                {
                    var claim = claims.Find(x => x.Type.Equals("supervisor"));
                    await _userManagerRepository.RemoveUserClaimAsync(user, claim);

                }
                var newClaim = new Claim("supervisor", createUserDto.Supervisor);
                await _userManagerRepository.CreateUserClaimAsync(user, newClaim);
            }

            if (createUserDto.Roles != null && createUserDto.Roles.Any()) 
            {
                if (claims != null && claims.Any(x => x.Type.Equals("role")))
                {
                    var claim = claims.Find(x => x.Type.Equals("role"));
                    await _userManagerRepository.RemoveUserClaimAsync(user, claim);
                }
                foreach (var role in createUserDto.Roles)
                {
                    
                    var newClaim = new Claim("role", role);
                    await _userManagerRepository.CreateUserClaimAsync(user, newClaim);
                   
                }

                await _userManagerRepository.DeleteUserRolesAsync(user, createUserDto.Roles);
                await _userManagerRepository.CreateUserRoleAsync(user, createUserDto.Roles);
            }

            if (!string.IsNullOrEmpty(createUserDto.Email) && createUserDto.Email != user.Email)
            {
                update = true;
                user.Email = createUserDto.Email;
            }

            if (user.IsActive != createUserDto.IsActive)
            {
                update = true;
                user.IsActive = createUserDto.IsActive;
            }

            if (!string.IsNullOrEmpty(createUserDto.Country) && createUserDto.Country != user.Country)
            {
                update = true;
                user.Country = createUserDto.Country;
            }

            if (createUserDto.IsActiveDirectory != user.ActiveDirectory)
            {
                update = true;
                user.ActiveDirectory = createUserDto.IsActiveDirectory;
            }

            if (update)
            {
                await _userManagerRepository.UpdateUserAsync(user);
            }
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManagerRepository.GetUserByUserNameAsync(changePasswordDto.UserName) ?? throw new Exception($"The username {changePasswordDto.UserName} does not exist");

            await _userManagerRepository.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        }

        public async Task<List<UserModel>> GetUserAsync(string? activeParam)
        {
            var users = activeParam switch
            {
                "1" => await _userManagerRepository.GetUserAsync(x => x.IsActive),
                "0" => await _userManagerRepository.GetUserAsync(y => !y.IsActive),
                "" => await _userManagerRepository.GetUserAsync(),
                null => await _userManagerRepository.GetUserAsync(),
                _ => await _userManagerRepository.GetUserAsync(),
            };

            if (users != null && users.Any())
            {
                var modelUser = _mapper.Map<List<UserModel>>(users);
                foreach (var model in modelUser)
                {
                    var user = (await _userManagerRepository.GetUserAsync(x => x.UserName.Equals(model.UserName))).FirstOrDefault();
                    var claims = await _userManagerRepository.GetUserCliamsAsync(user);
                    var roles = await _userManagerRepository.GetUserRoleAsync(user);

                    if (claims != null && claims.Any())
                    {
                        string? fullname = claims.Find(x => x.Type.Equals("name"))?.Value;
                        string? company = claims.Find(x => x.Type.Equals("companyid"))?.Value;
                        string? supervisor = claims.Find(x => x.Type.Equals("supervisor"))?.Value;
                        model.FullName = fullname;
                        model.Company = company;
                        model.Supervisor = supervisor;
                    }

                    model.Roles = roles.ToList();
                }

                return modelUser;
            }
            return new List<UserModel>();
        }

        public async Task<UserModel> GetUserByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("El parametro Id es requerido");

            var user = await _userManagerRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                var modelUser = _mapper.Map<UserModel>(user);
                var claims = await _userManagerRepository.GetUserCliamsAsync(user);
                var roles = await _userManagerRepository.GetUserRoleAsync(user);

                if (claims != null && claims.Any())
                {
                    string? fullname = claims.Find(x => x.Type.Equals("name"))?.Value;
                    string? company = claims.Find(x => x.Type.Equals("companyid"))?.Value;
                    string? supervisor = claims.Find(x => x.Type.Equals("supervisor"))?.Value;
                    modelUser.FullName = fullname;
                    modelUser.Company = company;
                    modelUser.Supervisor = supervisor;
                }

                modelUser.Roles = roles.ToList();

                return modelUser;
            }
            return new UserModel();
        }

        public async Task<List<UserModel>> GetSupervisor()
        {
            var users = await _userManagerRepository.GetUserAsync();

            if (users != null && users.Any())
            {
                var modelUser = _mapper.Map<List<UserModel>>(users);
                foreach (var model in modelUser)
                {
                    var user = (await _userManagerRepository.GetUserAsync(x => x.UserName.Equals(model.UserName))).FirstOrDefault();
                    var claims = await _userManagerRepository.GetUserCliamsAsync(user);
                    var roles = await _userManagerRepository.GetUserRoleAsync(user);

                    if (claims != null && claims.Any())
                    {
                        string? fullname = claims.Find(x => x.Type.Equals("name"))?.Value;
                        string? company = claims.Find(x => x.Type.Equals("companyid"))?.Value;
                        string? supervisor = claims.Find(x => x.Type.Equals("supervisor"))?.Value;
                        model.FullName = fullname;
                        model.Company = company;
                        model.Supervisor = supervisor;
                    }

                    model.Roles = roles.ToList();
                }

                return modelUser.Where(x => x.Roles.Contains("supervisor")).ToList();
            }
            return new List<UserModel>();
        }
    }
}
