namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public sealed record UserAuthenticateRequestV3
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Country { get; set; }
    public string? Tenant { get; set; }
    public bool EncryptPassword { get; set; } = false;
    public bool Parallel { get; set; } = false;
}
