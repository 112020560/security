namespace Akros.Authorizer.Application.Features.UserFeatures.Common;

public sealed record ErrorResponse()
{
    /// <summary>
    /// Para version 2 y 3
    /// </summary>
    public int statusCode { get; set; }
    public bool Islogged { get; set; }
    public string? Message { get; set; }
    /// <summary>
    /// Para la version 1
    /// </summary>
    public string? responseMessage { get; set; }
    public string? fullErrorMessage { get; set; }
    public int responseCode { get; set; }
    public DateTime loggedIn { get; set; } = DateTime.UtcNow;
}
