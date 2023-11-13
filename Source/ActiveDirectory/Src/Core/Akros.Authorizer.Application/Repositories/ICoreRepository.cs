using Akros.Authorizer.Domain.Entities.Persistence;

namespace Akros.Authorizer.Application.Repositories;

public interface ICoreRepository
{
    Task InsertLogedRegisterAsync(CoreUser model, string country);
}
