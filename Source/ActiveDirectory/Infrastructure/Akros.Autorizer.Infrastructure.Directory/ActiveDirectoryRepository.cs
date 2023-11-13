using Akros.Authorizer.Domain.Entities;
using Akros.Authorizer.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Akros.Authorizer.Infrastructure.Directory
{
    public class ActiveDirectoryRepository : IActiveDirectoryRepository
    {
        private readonly ILogger<ActiveDirectoryRepository> _logger;
        public ActiveDirectoryRepository(ILogger<ActiveDirectoryRepository> logger)
        {
            _logger = logger;
        }
        public async Task<UserLoggedEntity> Authenticate(UserModel userModel)
        {
            _logger.LogDebug("Begin authenticate in AD");

            string? username = userModel.UserName;
            string? password = userModel.Password;
            string? ldap = userModel.Ldap;

            try
            {
                if (string.IsNullOrEmpty(ldap)) throw new ArgumentNullException(nameof(ldap));
                if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
                if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

                var domainStr = await GetDomainInit(ldap);
                var domainAndUsername = $"{domainStr}\\{username}";

                using var entry = new DirectoryEntry(ldap, domainAndUsername, password);
                // var obj = entry.NativeObject;
                var search = new DirectorySearcher(entry)
                {
                    Filter = $"(SAMAccountName={username})"
                };
                search.PropertiesToLoad.Add("cn");
                var result = await Task.FromResult(search.FindOne());
                if (result != null)
                {
                    var pathLdap = result.Path;
                    string _filterAttribute = (string)result.Properties["cn"][0];
                    List<Rol> _groupsCurrentUser = await GetDomainGroupsUser(username, password, pathLdap, _filterAttribute);
                    //GetAttributes(username, password, pathLdap);
                    return new UserLoggedEntity
                    {
                        //ResponseCode = "OK",
                        IsLogged = true,
                        UserName = username,
                        Name = GetFullName(username, password, pathLdap, _filterAttribute),
                        Roles = _groupsCurrentUser
                    };
                }
                else
                {
                    return new UserLoggedEntity
                    {
                        IsLogged = false,
                        Message = $"No se encontraron registros para el usuario {username}"
                    };
                }

            }
            catch(Exception ex)
            {
                _logger.LogError("Usuario: {username} | LDAP:{ldap} | Error: Error al intentar auntenticar en ActiveDirectory", username, ldap);
                //throw new Exception("Credenciales inválidas");
                return new UserLoggedEntity
                {
                    IsLogged = false,
                    Message = "Credenciales inválidas",
                    Errors = new List<string> { $"Usuario: {username} | LDAP:{ldap} | Error: Error al intentar auntenticar en ActiveDirectory", ex.Message }
                };
            }
        }

        public async Task<bool> CreateUserAsync(CreateAdUserModel createAdUserModel)
        {
            DirectoryEntry adUserFolder = new DirectoryEntry(createAdUserModel.UserManagerAccess.LDap, createAdUserModel.UserManagerAccess.UserName, createAdUserModel.UserManagerAccess.Password);

            var accountname = createAdUserModel.UserName.Split('@')[0];

            if (adUserFolder.SchemaEntry.Name == "container")
            {
                DirectoryEntry newUser = adUserFolder.Children.Add("CN=" + createAdUserModel.UserName, "User");

                if (DirectoryEntry.Exists(newUser.Path)) throw new Exception("The username name already exists!");

                newUser.Properties["samAccountName"].Value = accountname;
                newUser.Properties["givenName"].Value = createAdUserModel.FirstName;
                newUser.Properties["sn"].Value = createAdUserModel.LastName;
                newUser.Properties["initials"].Value = createAdUserModel.FirstName.Substring(0, 1);
                newUser.Properties["displayName"].Value = $"{createAdUserModel.FirstName} {createAdUserModel.LastName}";
                newUser.Properties["physicalDeliveryOfficeName"].Value = createAdUserModel.OfficeName;
                newUser.Properties["telephoneNumber"].Value = createAdUserModel.PhoneNumber;
                newUser.Properties["homePhone"].Value = createAdUserModel.Identification;
                newUser.Properties["mail"].Value = createAdUserModel.UserName;

                newUser.CommitChanges();
                newUser.Invoke("setpassword", createAdUserModel);
                newUser.Properties["userAccountControl"].Value = 0x0200;
                newUser.CommitChanges();

                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        private async Task<string> GetDomainInit(string path)
        {
            var dmn = path.Split(new char[] { '/' });
            var starIndex = path.IndexOf("/", 1);
            var endIndex = path.IndexOf(".", 1);
            var domain = await Task.FromResult(ValidateIp(dmn[2]) ? dmn[2] : path.Substring(starIndex + 2, endIndex - 2 - starIndex));
            //path.Substring(starIndex + 2, endIndex - 2 - starIndex);
            return domain;
        }

        private bool ValidateIp(string sIP)
        {
            try
            { IPAddress ip = IPAddress.Parse(sIP); }
            catch
            { return false; }

            return true;
        }

        private async Task<List<Rol>> GetDomainGroupsUser(string username, string password, string pathLdap, string filterAttribute)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("string Username, string Password, string PathLdap");
                }
                var entry = new DirectoryEntry(pathLdap, username, password);
                var search = new DirectorySearcher(entry);
                search.Filter = "(cn=" + filterAttribute + ")";
                search.PropertiesToLoad.Add("memberOf");
                var result = search.FindOne();
                if (result != null)
                {
                    var propertyCount = result.Properties["memberOf"].Count;
                    var listGroups = new List<Rol>();

                    for (var propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                    {
                        var dn = (string)result.Properties["memberOf"][propertyCounter];
                        var equalsIndex = dn.IndexOf("=", 1, StringComparison.Ordinal);
                        var commaIndex = dn.IndexOf(",", 1, StringComparison.Ordinal);

                        if (-1 == equalsIndex)
                            return listGroups;

                        var group = new StringBuilder();
                        group.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                        listGroups.Add(new Rol { UserRol = group.ToString() });
                    }

                    return await Task.FromResult(listGroups);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo los grupos del usuario. [{ex.Message}]");
            }
        }

        private string GetFullName(string username, string password, string pathLdap, string filterAttribute)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("Username&Password&PathLdap");
                }
                var entry = new DirectoryEntry(pathLdap, username, password);
                var search = new DirectorySearcher(entry);
                search.Filter = "(cn=" + filterAttribute + ")";
                search.PropertiesToLoad.Add("fullName");
                var result = search.FindOne();
                var path = result.Path;
                var starIndex = path.IndexOf("=", 1);
                var endIndex = path.IndexOf(",", 1);
                var fullName = path.Substring(starIndex + 1, endIndex - 1 - starIndex);
                return fullName;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void GetAttributes(string username, string password, string pathLdap)
        {
            DirectoryEntry deTargetUser = new DirectoryEntry(pathLdap, username, password);
            DirectorySearcher dsSchema = new DirectorySearcher(deTargetUser)
            {
                SearchScope = SearchScope.Base, //allowedAttributes is a constructed attribute, so have to ask for it while performing a search
                Filter = "(objectClass=*)" //this is closest thing I can find to an always true filter
            };
            dsSchema.PropertiesToLoad.Add("homePhone");
            foreach (SearchResult res in dsSchema.FindAll())
            {
                _logger.LogInformation("Identificacion Usuario: {id}", res.Properties["homePhone"][0]);
            }
            //SearchResult srSchema = dsSchema.FindAll();
            //var attributes = new List<string>();
            //foreach (string attributeName in srSchema.Properties["allowedAttributes"])
            //{
            //    if(attributeName.Equals("homePhone"))
            //    {

            //    }
            //    attributes.Add(attributeName);
            //}
        }

    }
}
