using Akros.Authorizer.Application.Common.Exceptions;
using Akros.Authorizer.Application.Features.UserFeatures.Common.Interfaces;
using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Persistence;
using Akros.Authorizer.Domain.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public class UserAuthenticateService : IUserAuthenticateService
{
    private readonly ILogger<UserAuthenticateService> _logger;
    private readonly IEncryptRepository _encryptRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IActiveDirectoryRepository _activeDirectoryRepository;
    private readonly ICoreService _coreRepository;
    private readonly ILoggedCacheService _loggedCacheService;
    private readonly IMapper _mapper;
    private readonly string _traceId;
    private const string LDAP_DEFAULT = @"LDAP://Akros.local/DC=Akros, DC=local|LDAP://cr.f2p.in/DC=cr, DC=f2p, DC=in|LDAP://ni.f2p.in/DC=ni, DC=f2p, DC=in|LDAP://gt.f2p.in/DC=gt, DC=f2p, DC=in|LDAP://172.24.10.41/DC=sv, DC=f2p, DC=in";
    public UserAuthenticateService(IEncryptRepository encryptRepository,
                                   ICountryRepository countryRepository,
                                   IActiveDirectoryRepository activeDirectoryRepository,
                                   IMapper mapper,
                                   ILogger<UserAuthenticateService> logger,
                                   IContextTraceRepository contextTraceRepository,
                                   ICoreService coreRepository,
                                   ILoggedCacheService loggedCacheService)
    {
        _encryptRepository = encryptRepository;
        _countryRepository = countryRepository;
        _activeDirectoryRepository = activeDirectoryRepository;
        _mapper = mapper;
        _logger = logger;
        _traceId = contextTraceRepository.GetTraceId().ToString();
        _coreRepository = coreRepository;
        _loggedCacheService = loggedCacheService;
    }
    public async Task<UserAuthenticateResponse> ExecuteAsync(UserAuthenticateRequest request,string? decryptKey)
    {
        _logger.LogInformation("{traceid} - Authenticate user: {username}", _traceId, request.User);

        ///VERIFICAMOS SI EL PASSWORD VIENE ENCRIPTADO
        string? password = request.Password;
        if (request.Encrypt.Equals(1))
        {
            _logger.LogDebug("{traceid} - Authtenticate has encrypted password", _traceId);

            if (string.IsNullOrEmpty(decryptKey)) throw new ArgumentNullException(nameof(decryptKey)); //TODO:: Retornar error de validacion

            password = _encryptRepository.DecryptString(request.Password ?? string.Empty, decryptKey);
        }
        request.Password = password;

        ///MAPEAMOS EL REQUEST A LA ENTIDAD 
        var entity = _mapper.Map<User>(request);
     
        
        ///AUTENTICAMOS EN EL ACTIVE DIRECTORY
        var userInformation = await this.AuthenticateAsync(entity, request.Cod_Pais, "v1");
        return new UserAuthenticateResponse
        {
            Islogged = true,
            LastLogon = userInformation.LastLogon,
            Name = userInformation.Name,
            Password = string.Empty,
            ResponseCode = "OK",
            ResponseMessage = $"User {userInformation.Username} successfully authenticated",
            Roles = userInformation.Roles,
            StrRoles = userInformation.StrRoles,
            Username = userInformation.Username,
        };
    }

    public async Task<UserAuthenticateResponseV2> ExecuteV2Async(UserAuthenticateRequest request, string? decryptKey)
    {
        ///VERIFICAMOS SI EL PASSWORD VIENE ENCRIPTADO
        string? password = request.Password;
        if (request.Encrypt.Equals(1))
        {
            _logger.LogDebug("{traceid} - Authtenticate has encrypted password", _traceId);

            if (string.IsNullOrEmpty(decryptKey)) throw new ArgumentNullException(nameof(decryptKey)); //TODO:: Retornar error de validacion

            password = _encryptRepository.DecryptString(request.Password ?? string.Empty, decryptKey);
        }
        request.Password = password;

        ///MAPEAMOS EL REQUEST A LA ENTIDAD 
        var entity = _mapper.Map<User>(request);

        ///AUTENTICAMOS EN EL ACTIVE DIRECTORY
        var userInformation = await this.AuthenticateAsync(entity, request.Cod_Pais, "v2");
        return new UserAuthenticateResponseV2
        {
            responseCode = "00",
            responseData = new
            {
                Isloged = true,
                userInformation.Username,
                Password = string.Empty,
                userInformation.Name,
                Roles = userInformation?.Roles != null ? userInformation?.Roles.Select(x => new Rol { UserRol = x }).ToList() : new List<Rol>(),
            }
        };
    }

    public async Task<UserAuthenticateResponseV3> ExecuteV3Async(UserAuthenticateRequestV3 request, string? decryptKey)
    {
        ///VERIFICAMOS SI EL PASSWORD VIENE ENCRIPTADO
        string? password = request.Password;
        if (request.EncryptPassword)
        {
            _logger.LogDebug("{traceid} - Authtenticate has encrypted password", _traceId);

            if (string.IsNullOrEmpty(decryptKey)) throw new ArgumentNullException(nameof(decryptKey)); //TODO:: Retornar error de validacion

            password = _encryptRepository.DecryptString(request.Password ?? string.Empty, decryptKey);
        }
        request.Password = password;
        ///MAPEAMOS EL REQUEST A LA ENTIDAD 
        var entity = _mapper.Map<User>(request);

        ///AUTENTICAMOS EN EL ACTIVE DIRECTORY
        var userInformation = await this.AuthenticateAsync(entity, request.Country, "v3");
        return new UserAuthenticateResponseV3
        {
            Islogged = true,
            loggedIn = DateTime.UtcNow,
            Message = $"User {userInformation.Username} successfully authenticated",
            UserInformation = new Common.UserInformationResponse
            {
                Username = userInformation.Username,
                Email = userInformation.Email,
                LastLogon = userInformation?.LastLogon,
                Name = userInformation?.Name,
                Password = string.Empty,
                Roles = userInformation?.Roles,
                StrRoles = userInformation?.StrRoles,
            }
        };
    }

    /*
     * *********************************************************************
     * METODOS PRIVADOS
     *  *********************************************************************
     */
    private async Task<UserInformationModel> AuthenticateAsync(User user, string? codCountry, string version)
    {
        var ldapList = await GetLdapsConfigurationsAsync(codCountry);
        if (ldapList != null && ldapList.Any())
        {
            user.LDAPs = ldapList;
            var (authenticate, userInformation) = await _activeDirectoryRepository.ActiveDirectoryAuthenticateAsync(user);
            if (authenticate)
            {
                if (string.IsNullOrEmpty(user.UserName)) throw new ArgumentNullException(nameof(user.UserName));
                ///OBTENEMOS EL PAIS
                var country = codCountry ?? (await _loggedCacheService.GetCacheUserCountry(user.UserName)) ?? userInformation.Country ?? "CR";
                ///EJECUTAMOS LAS ACCIONES ASINCRONICAS
                ///REGISTRO EN EL CORE
                await this._coreRepository.InsertCoreRegisterAsync(userInformation, country);
                ///REGISTRO EN MONGO
                await this._loggedCacheService.CreateLoggedCacheAsync(user.UserName, country, version, userInformation.ToString());
                ///PROCESAMOS LA RESPUESTA
                return userInformation;

            }
            throw new UnauthorizedException($"Login Failed for user {user.UserName} at {DateTime.UtcNow}");
        }
        throw new ApiException($"Not LDAP configure to country");
    }


    private async Task<string[]> GetLdapsConfigurationsAsync(string? country)
    {
        _logger.LogDebug("{traceid} - Get LdapList", _traceId);
        string[] ldaps;
        if (!string.IsNullOrEmpty(country))
        {
            var country_ldpa = await _countryRepository.GetLdpaByCountryAsync(country);
            if (!string.IsNullOrEmpty(country_ldpa))
            {
                ldaps = country_ldpa.Split("|");
            }
            else
            {
                ///ESTO DEBERIA DE IR EN EL CONFIG PERO NO DEBE DE FALLAR POR FALTA DE CONFIGURACION
                ldaps = LDAP_DEFAULT.Split("|");
            }
        }
        else
        {
            var country_ldpa = await _countryRepository.GetLdpaByCountryAsync("REG");
            if (!string.IsNullOrEmpty(country_ldpa))
            {
                ldaps = country_ldpa.Split("|");
            }
            else
            {
                ldaps = LDAP_DEFAULT.Split("|");
            }
        }
        _logger.LogDebug("{traceid} - LdapList {ldaps}", _traceId, ldaps);
        return ldaps ?? Array.Empty<string>();
    }



    //private dynamic ProcessResponse(string version, UserInformationModel userInformation)
    //{
    //    switch (version)
    //    {
    //        default:
    //        case "v1":
    //            return new UserAuthenticateResponse
    //            {
    //                Islogged = true,
    //                LastLogon = userInformation.LastLogon,
    //                Name = userInformation.Name,
    //                Password = string.Empty,
    //                ResponseCode = "OK",
    //                ResponseMessage = $"User {userInformation.Username} successfully authenticated",
    //                Roles = userInformation.Roles,
    //                StrRoles = userInformation.StrRoles,
    //                Username = userInformation.Username,
    //            };

    //        case "v2":
    //            return new UserAuthenticateResponseV2
    //            {
    //                responseCode = "00",
    //                responseData = new
    //                {
    //                    Isloged = true,
    //                    userInformation.Username,
    //                    Password = string.Empty,
    //                    userInformation.Name,
    //                    Roles = userInformation?.Roles != null ? userInformation?.Roles.Select(x => new Rol { UserRol = x }).ToList() : new List<Rol>(),
    //                }
    //            };

    //        case "v3":
    //            return new UserAuthenticateResponseV3
    //            {
    //                Islogged = true,
    //                loggedIn = DateTime.UtcNow,
    //                Message = $"User {userInformation.Username} successfully authenticated",
    //                UserInformation = new Common.UserInformationResponse
    //                {
    //                    Username = userInformation.Username,
    //                    Email = userInformation.Email,
    //                    LastLogon = userInformation?.LastLogon,
    //                    Name = userInformation?.Name,
    //                    Password = string.Empty,
    //                    Roles = userInformation?.Roles,
    //                    StrRoles = userInformation?.StrRoles,
    //                }
    //            };
    //    }
    //}
}
