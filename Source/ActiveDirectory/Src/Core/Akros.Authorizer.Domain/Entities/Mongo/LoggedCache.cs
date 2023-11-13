using Akros.Authorizer.Domain.Common;

namespace Akros.Authorizer.Domain.Entities.Mongo;

[BsonCollection("LoggedCache")]
public sealed class LoggedCache: Document
{
    public string? UserName { get; set; }
    public string? Country { get; set; }
    public string? VersionEndpoint { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CurrentLogin { get; set; }
    public string? AdditionalData { get; set; }
}
