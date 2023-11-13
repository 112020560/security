namespace Akros.Authorizer.Application.Features.UserFeatures.UserAuthenticate;

public sealed record UserAuthenticateResponseV2
{
    public string? responseCode { get; set; }
    public object? responseData { get; set; }
    public string? responseMessage { get; set; }
}
