namespace Akros.Authorizer.Application.Features.UserFeatures.GetUsers;

public interface IGetUsersService
{
    Task<List<GetUserResponse>> ExecuteAsync(string country);
}
