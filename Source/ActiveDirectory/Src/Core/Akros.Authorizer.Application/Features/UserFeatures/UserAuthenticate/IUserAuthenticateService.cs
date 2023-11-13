namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public interface IUserAuthenticateService
{
    //Task<UserAuthenticateResponse> ExecuteAsync(UserAuthenticateRequest request, string? decryptKey);
    //Task<dynamic> ExecuteAsync<T>(UserAuthenticateRequest request, string version, string? decryptKey);
    /// <summary>
    /// METODO ENCARGADO DE AUTENTICAR EN VERSION 1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="decryptKey"></param>
    /// <returns>UserAuthenticateResponse</returns>
    Task<UserAuthenticateResponse> ExecuteAsync(UserAuthenticateRequest request, string? decryptKey);

    /// <summary>
    /// METODO ENCARGADO DE AUTENTICAR EN VERSION 2
    /// </summary>
    /// <param name="request"></param>
    /// <param name="decryptKey"></param>
    /// <returns>UserAuthenticateResponseV2</returns>
    Task<UserAuthenticateResponseV2> ExecuteV2Async(UserAuthenticateRequest request, string? decryptKey);

    /// <summary>
    /// METODO ENCARGADO DE AUTENTICAR EN VERSION 3
    /// </summary>
    /// <param name="request"></param>
    /// <param name="decryptKey"></param>
    /// <returns>UserAuthenticateResponseV3</returns>
    Task<UserAuthenticateResponseV3> ExecuteV3Async(UserAuthenticateRequestV3 request, string? decryptKey);
}
