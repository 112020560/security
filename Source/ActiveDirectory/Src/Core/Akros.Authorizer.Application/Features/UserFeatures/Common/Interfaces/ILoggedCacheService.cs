namespace Akros.Authorizer.Application.Features.UserFeatures.Common.Interfaces;

public interface ILoggedCacheService
{
    Task CreateLoggedCacheAsync(string userName, string country, string version, string additionalData);
    Task<string?> GetCacheUserCountry(string username);
}
