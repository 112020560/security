using Akros.Authorizer.Domain.Entities.Mongo;
using Akros.Authorizer.Domain.Entities.Persistence;

namespace Akros.Authorizer.Application.Repositories;

public interface ICountryRepository: IMongoRepository<Country>
{
    Task<string?> GetLdpaByCountryAsync(string country);
    Task<List<DomainModel>> GetDomainByContry();
}
