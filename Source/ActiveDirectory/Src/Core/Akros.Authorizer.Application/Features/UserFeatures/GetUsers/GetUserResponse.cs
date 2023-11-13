namespace Akros.Authorizer.Application.Features.UserFeatures.GetUsers;

public sealed record GetUserResponse
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public bool isMapped { get; set; }
}
