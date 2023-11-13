using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Mongo;
using Akros.Authorizer.Domain.Settings;

namespace Akros.Authorizer.Infrastructure.Mongo.Repositories;

public sealed class LoggedCacheRepository : MongoRepository<LoggedCache>, ILoggedCacheRepository
{
    public LoggedCacheRepository(IMongoDbSettings settings) : base(settings)
    {
    }


    public async Task CreateLoggedCacheAsync(LoggedCache loggedCache)
    {
        var cache = await FindOneAsync(x => x.UserName == loggedCache.UserName);
        if (cache == null) 
        { 
            await InsertOneAsync(loggedCache);
        }
        else
        {
            cache.LastLogin = loggedCache.CurrentLogin;
            cache.CurrentLogin = DateTime.UtcNow;
            cache.VersionEndpoint = loggedCache.VersionEndpoint;
            cache.AdditionalData = loggedCache.AdditionalData;

            await ReplaceOneAsync(cache);
        }
    }
}
