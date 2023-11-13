using Akros.Authorizer.Application.Features.UserFeatures.Common;

namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public sealed record UserAuthenticateResponseV3
{
    public bool Islogged { get; set; }
    public string? Message { get; set; }
    public DateTime loggedIn { get; set; } = DateTime.UtcNow;
    public UserInformationResponse? UserInformation { get; set; }
}
