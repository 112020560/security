namespace Akros.Authorizer.Application.Features.Listeners.UserCoreRegister
{
    public sealed record CacheLoggedContract
    {
        public string? UserName { get; set; }
        public string? Country { get; set; }
        public string? VersionEndpoint { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CurrentLogin { get; set; }
        public string? AdditionalData { get; set; }
    }
}
