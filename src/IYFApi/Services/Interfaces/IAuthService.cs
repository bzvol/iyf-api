using FirebaseAdmin.Auth;

namespace IYFApi.Services.Interfaces;

public interface IAuthService
{
    public Task<IEnumerable<UserRecord>> GetAllUsers();
    public Task SetDefaults(string uid);
    public Task RequestAccess(string uid);
    public Task GrantAccess(string uid);
}