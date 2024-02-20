using FirebaseAdmin.Auth;
using IYFApi.Models.Request;

namespace IYFApi.Services.Interfaces;

public interface IAuthService
{
    public Task<IEnumerable<UserRecord>> GetAllUsersAsync();
    public Task<UserRecord> GetUserAsync(string uid);
    public Task SetDefaultCustomClaimsAsync(string uid);
    public Task ClearCustomClaimsAsync(string uid);
    public Task RequestAccessAsync(string uid);
    public Task GrantAccessAsync(string uid, bool grant);
    public Task RevokeAccessAsync(string uid, bool notifyUser);
    public Task UpdateRolesAsync(string uid, UpdateRolesRequest request);
}