namespace Akros.Authorizer.Application.Features.UserFeatures.Common;

public sealed record UserInformationResponse
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public List<string>? Roles { get; set; }
    public string? StrRoles { get; set; }
    public string? Email { get; set; }
    public string? LastLogon { get; set; }
}
