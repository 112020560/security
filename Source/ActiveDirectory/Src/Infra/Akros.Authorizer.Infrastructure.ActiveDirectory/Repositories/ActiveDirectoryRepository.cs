using Akros.Authorizer.Application.Common.Exceptions;
using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Persistence;
using Akros.Authorizer.Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Text;

namespace Akros.Authorizer.Infrastructure.ActiveDirectory.Repositories;

public sealed class ActiveDirectoryRepository: IActiveDirectoryRepository
{
    private readonly ILogger<ActiveDirectoryRepository> _logger;
    private string? _filterAttribute;
    private List<string>? _groupsCurrentUser;
    private readonly string _traceId;
    public ActiveDirectoryRepository(ILogger<ActiveDirectoryRepository> logger, IContextTraceRepository contextTraceRepository)
    {
        _logger = logger;
        _traceId = contextTraceRepository.GetTraceId().ToString();
    }
    public async Task<(bool authenticate, UserInformationModel userInformation)> ActiveDirectoryAuthenticateAsync(User userEntity)
    {
        _logger.LogDebug("{traceid} - Begin authenticate in AD", _traceId);
        var strErrorLog = new StringBuilder();
        var arrayPathLdap = userEntity.LDAPs;

        if (arrayPathLdap != null && arrayPathLdap.Any())
        {
            foreach (var ldap in arrayPathLdap)
            {
                _logger.LogDebug("{traceid} - Procesando LDAP {ldap}", _traceId, ldap);
                try
                {
                    if (!string.IsNullOrEmpty(ldap))
                    {
                        if (string.IsNullOrEmpty(userEntity.UserName) || string.IsNullOrEmpty(userEntity.Password))
                        {
                            throw new ArgumentNullException("UserName && Password");
                        }
                        var domainStr = GetDomainInit(ldap);
                        var domainAndUsername = $"{domainStr}\\{userEntity.UserName}";

                        _logger.LogDebug("{traceid} - domainStr {domainStr}", _traceId, domainStr);
                        _logger.LogDebug("{traceid} - domainAndUsername {domainAndUsername}", _traceId,domainAndUsername);
                        _logger.LogDebug("{traceid} - Password {domainAndUsername}", _traceId,userEntity.Password);

                        using (var entry = new DirectoryEntry(ldap, domainAndUsername, userEntity.Password))
                        {
                            _logger.LogDebug("{traceid} - response Entry {entry}", _traceId,JsonConvert.SerializeObject(entry));

                            // var obj = entry.NativeObject;
                            var search = new DirectorySearcher(entry)
                            {
                                Filter = $"(SAMAccountName={userEntity.UserName})"
                            };
                            search.PropertiesToLoad.Add("cn");
                            var result = search.FindOne();

                            _logger.LogDebug("{traceid} - response Result {entry}", _traceId,JsonConvert.SerializeObject(result));

                            if (result != null)
                            {
                                var pathLdap = result.Path;
                                _filterAttribute = (string)result.Properties["cn"][0];
                                _groupsCurrentUser = GetDomainGroupsUser(userEntity.UserName, userEntity.Password, pathLdap);
                                var roles = _groupsCurrentUser;
                                var user = new UserInformationModel
                                {
                                    //ResponseCode = "OK",
                                    Username = userEntity.UserName,
                                    Password = userEntity.Password,
                                    Name = GetFullName(userEntity.UserName, userEntity.Password, pathLdap),
                                    Roles = roles ?? new List<string>(),
                                    Email = string.Empty,
                                    StrRoles = String.Join("|", roles ?? new List<string>()),
                                    Country = this.GetCountry(ldap)
                                    //$"{userEntity.UserName}@"
                                    //GetEmail(domainAndUsername, userEntity.UserName, userEntity.Password, pathLdap)
                                };
                                var tcs = new TaskCompletionSource<(bool, UserInformationModel)>();
                                tcs.SetResult((true, user));
                                return await tcs.Task;
                            }
                        }
                        strErrorLog.Append($"Usuario: {userEntity.UserName} | LDAP:{ldap} | Error: Error al intentar auntenticar en ActiveDirectory");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{traceid} - Error {ex}", _traceId,ex.Message);
                    strErrorLog.Append($" LDAP:{ldap} | Usuario: {userEntity.UserName} => Error: {ex.Message}");
                }

            }//foreach
        }
        else
        {
            strErrorLog.AppendLine("The LDAP list is empty");
        }

        if (!string.IsNullOrEmpty(strErrorLog.ToString()))
        {
            _logger.LogInformation(strErrorLog.ToString().Trim());
        }

        throw new UnauthorizedException("Credenciales inválidas");
    }

    public async Task<List<User>> GetADUsers(string[] ldaps)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuarios del dominio");

            List<User> lstADUsers = new();
            foreach (var ldap in ldaps)
            {
                _logger.LogDebug("Buscando usuarios en {ldap}", ldap);

                string DomainPath = ldap;
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = "(&(objectClass=user)(objectCategory=person)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
                search.PropertiesToLoad.Add("samaccountname");
                //search.PropertiesToLoad.Add("mail");
                //search.PropertiesToLoad.Add("usergroup");
                //search.PropertiesToLoad.Add("displayname");//first name
                search.PageSize = 100000;
                SearchResult result;
                SearchResultCollection resultCol = await Task.FromResult(search.FindAll());
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname"))
                        {
                            User objSurveyUsers = new();
                            objSurveyUsers.Email = result.Properties.Contains("mail") ? (String)result.Properties["mail"][0] : "N/D";
                            objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
                            objSurveyUsers.DisplayName = result.Properties.Contains("displayname") ? (String)result.Properties["displayname"][0] : "N/D";
                            lstADUsers.Add(objSurveyUsers);
                        }
                    }
                }
            }
            return lstADUsers;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "No fue posible obtener los usuarios: {error}", ex.Message);
            return new List<User>();
        }
    }

    private string GetFullName(string username, string password, string pathLdap)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Username&Password&PathLdap");
            }
            var entry = new DirectoryEntry(pathLdap, username, password);
            var search = new DirectorySearcher(entry);
            search.Filter = "(cn=" + this._filterAttribute + ")";
            search.PropertiesToLoad.Add("fullName");
            var result = search.FindOne();
            var path = result.Path;
            var starIndex = path.IndexOf("=", 1);
            var endIndex = path.IndexOf(",", 1);
            var fullName = path.Substring(starIndex + 1, endIndex - 1 - starIndex);
            return fullName;
        }
        catch (DirectoryException ex)
        {
            throw new Exception($"Error al obtener el usuario {ex.Message}");
        }
    }

    private string GetDomainInit(string path)
    {
        var dmn = path.Split(new char[] { '/' });
        var starIndex = path.IndexOf("/", 1);
        var endIndex = path.IndexOf(".", 1);
        var domain = ValidateIp(dmn[2]) ? dmn[2] : path.Substring(starIndex + 2, endIndex - 2 - starIndex);
        //path.Substring(starIndex + 2, endIndex - 2 - starIndex);
        return domain;
    }

    private string GetCountry(string path)
    {
        try
        {
            var dmn = path.Split(new char[] { '/' });
            if (dmn != null && dmn.Length > 0)
            {
                var country = dmn[3].Split(",")[0][3..];
                if (country.ToUpper() == "AKROS") return "CR";
                return country;
            }
            return "CR";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al extraer el country {ex.Message}, se devuelve el pais por defecto CR");
            return "CR";
        }
        
    }

    private bool ValidateIp(string sIP)
    {
        try
        { IPAddress ip = IPAddress.Parse(sIP); }
        catch
        { return false; }

        return true;
    }

    private List<string> GetDomainGroupsUser(string username, string password, string pathLdap)
    {
        _logger.LogDebug("Obteniendo Roles");
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("string Username, string Password, string PathLdap");
            }
            var entry = new DirectoryEntry(pathLdap, username, password);
            var search = new DirectorySearcher(entry);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            var result = search.FindOne();

            _logger.LogDebug("response result {result}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result != null)
            {
                var propertyCount = result.Properties["memberOf"].Count;
                var listGroups = new List<string>();

                for (var propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    var dn = (string)result.Properties["memberOf"][propertyCounter];
                    var equalsIndex = dn.IndexOf("=", 1, StringComparison.Ordinal);
                    var commaIndex = dn.IndexOf(",", 1, StringComparison.Ordinal);

                    if (-1 == equalsIndex)
                        return listGroups;

                    var group = new StringBuilder();
                    group.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    listGroups.Add(group.ToString());
                }

                return listGroups;
            }

            return new List<string>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error obteniendo los grupos del usuario. [{ex.Message}]");
        }
    }
}
