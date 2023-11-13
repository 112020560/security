using Akros.Authorizer.Application.Repositories;
using Akros.Authorizer.Domain.Entities.Persistence;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Akros.Authorizer.Infrastructure.Persistence.Repositories;

public sealed class CoreRepository: ICoreRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CoreRepository> _logger;
    public CoreRepository(IConfiguration configuration, ILogger<CoreRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public async Task InsertLogedRegisterAsync(CoreUser model, string country)
    {
        var connecionString = _configuration.GetConnectionString("CORE") ?? throw new Exception("The connectionString is null or empty");
        using SqlConnection conn = new(connecionString.Replace("[XX]", country));
        try
        {
            _logger.LogInformation("REGISTRANDO USUARIO EN EL CORE");
            conn.Open();
            await conn.ExecuteAsync("PA_MAN_STC_SYS_USER", model, commandType: CommandType.StoredProcedure);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "ERROR AL REGISTRAR EL USUARIO EN EL CORE: {error}", ex.Message);
        }
        finally
        {
            _logger.LogInformation("FIN REGISTRO USUARIO EN EL CORE");
            conn.Close();
        }
    }
}
