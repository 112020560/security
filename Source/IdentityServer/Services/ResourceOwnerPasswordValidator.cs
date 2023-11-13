using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;
using System.Text;

namespace IdentityServer.Services;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<ResourceOwnerPasswordValidator> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null && user.IsActive)
            {
                if (user.ActiveDirectory)
                {
                    var payload = new
                    {
                        userName = context.UserName,
                        password = context.Password,
                        country = user.Country,
                    };

                    using HttpClient httpClient = new();
                    httpClient.BaseAddress = new Uri(_configuration["ActiveDirectory:BaseUrl"]);
                    HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, _configuration["ActiveDirectory:EndPoint"])
                    {
                        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                    };
                    httpRequestMessage.Headers.Add("X-Api-Version", "1");

                    var ad_response = await httpClient.SendAsync(httpRequestMessage);
                    if (ad_response.IsSuccessStatusCode)
                    {
                        var response = await ad_response.Content.ReadAsStringAsync();
                        var data = System.Text.Json.JsonSerializer.Deserialize<UserLoggedEntity>(response, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true});
                        if (data.isLogged)
                        {
                            context.Result = new GrantValidationResult(
                                subject: user.Id.ToString(),
                                authenticationMethod: "custom",
                                claims: await _userManager.GetClaimsAsync(user));
                            return;
                        }
                        else
                        {
                            context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient, $"Active Directory Error: {data.message}");
                            return;
                        }
                    }
                    else
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.UnauthorizedClient, $"Active Directory {httpRequestMessage.RequestUri} Error : {ad_response.ReasonPhrase}");
                        return;
                    }
                }
                else
                {
                    if (await _userManager.CheckPasswordAsync(user, context.Password))
                    {
                        context.Result = new GrantValidationResult(
                                subject: user.Id.ToString(),
                                authenticationMethod: "custom",
                                claims: await _userManager.GetClaimsAsync(user));
                        return;
                    }
                    else
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                        return;
                    }
                }

            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                return;
            }
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            Console.WriteLine(ex);
        }
    }
}

public class UserLoggedEntity
{
    public bool isLogged { get; set; }
    public string? userName { get; set; }
    public string? name { get; set; }
    public List<Rol>? roles { get; set; }
    public string? message { get; set; }
    public List<string> errors { get; set; }

}

public class Rol
{
    public string? userRol { get; set; }
}
