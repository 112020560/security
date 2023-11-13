using Akros.Authorizer.Application.Features.UserFeatures.Common.Interfaces;
using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Mongo;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Akros.Authorizer.Application.Features.UserFeatures.Common
{
    public sealed class LoggedCacheService: ILoggedCacheService
    {
        private readonly IBrockerProducerRepository _brockerProducerRepository;
        private readonly ILogger<LoggedCacheService> _logger;
        private readonly ILoggedCacheRepository _loggedCacheRepository;
        public LoggedCacheService(IBrockerProducerRepository brockerProducerRepository, ILogger<LoggedCacheService> logger, ILoggedCacheRepository loggedCacheRepository)
        {
            _brockerProducerRepository = brockerProducerRepository;
            _logger = logger;
            _loggedCacheRepository = loggedCacheRepository;
        }
        public async Task CreateLoggedCacheAsync(string userName, string country, string version, string additionalData)
        {
            var cache = new LoggedCache
            {
                AdditionalData = additionalData,
                UserName = userName,
                Country = country,
                CurrentLogin = DateTime.Now,
                VersionEndpoint = version,
                LastLogin = DateTime.Now,
            };

            var strData = JsonConvert.SerializeObject(cache);
            _logger.LogDebug("{username} - Informacion que se enviara al CACHE [MongoDB]: {data}", userName, strData);

            await _brockerProducerRepository.SendMongoLoggdRegisterAsync(strData, "akros.authenticate", "register.logged.cache");
        }

        public async Task<string?> GetCacheUserCountry(string username)
        {
            var cache = await _loggedCacheRepository.FindOneAsync(c => c.UserName == username);
            if(cache != null)
            {
                return cache.Country;
            }

            return null;
        }
    }
}
