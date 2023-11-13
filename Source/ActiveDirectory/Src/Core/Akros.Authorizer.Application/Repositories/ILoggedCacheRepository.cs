using Akros.Authorizer.Domain.Entities.Mongo;

namespace Akros.Authorizer.Application.Repositories;

public interface ILoggedCacheRepository: IMongoRepository<LoggedCache>
{
    Task CreateLoggedCacheAsync(LoggedCache loggedCache);
}
