using Akros.Authorizer.Domain.Models;

namespace Akros.Authorizer.Application.Features.UserFeatures.Common.Interfaces;

public interface ICoreService
{
    Task InsertCoreRegisterAsync(UserInformationModel model, string country);
}
