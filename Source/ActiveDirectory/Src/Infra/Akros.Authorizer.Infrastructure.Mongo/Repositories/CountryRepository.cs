using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Mongo;
using Akros.Authorizer.Domain.Entities.Persistence;
using Akros.Authorizer.Domain.Settings;

namespace Akros.Authorizer.Infrastructure.Mongo.Repositories;

public sealed class CountryRepository : MongoRepository<Country>, ICountryRepository
{
    public CountryRepository(IMongoDbSettings settings) : base(settings)
    {
    }

    public async Task<string?> GetLdpaByCountryAsync(string country)
    {
        var selected_country = await FindOneAsync(x => x.abbreviation == country);
        if (selected_country == null) return null;
        return selected_country.ldap;
    }

    public async Task<List<DomainModel>> GetDomainByContry()
    {
        var countries =  AsQueryable().ToList();
        var domains = countries.Select(c => new DomainModel { Domain = c.domain }).ToList();
        return await Task.FromResult(domains);
    }
}
