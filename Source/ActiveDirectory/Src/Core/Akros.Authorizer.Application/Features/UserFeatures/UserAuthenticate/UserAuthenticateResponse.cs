using Akros.Authorizer.Application.Features.UserFeatures.Common;

namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public sealed record UserAuthenticateResponse()
{
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }

    public bool Islogged { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public List<string>? Roles { get; set; }
    public string? StrRoles { get; set; }
    public string? Email { get; set; }
    public string? LastLogon { get; set; }

    public string? Message { get; set; }
    public DateTime LoggedIn { get; set; } = DateTime.UtcNow;
}
