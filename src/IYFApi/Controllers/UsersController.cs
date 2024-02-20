using FirebaseAdmin.Auth;
using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class UsersController(IAuthService authService) : ControllerBase
{
    [HttpGet]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task<IEnumerable<UserRecord>> GetAllUsers() => await authService.GetAllUsersAsync();
    
    [HttpGet("{uid}")]
    public async Task<UserRecord> GetUser(string uid) => await authService.GetUserAsync(uid);
    
    [HttpPost("{uid}/set-default-claims")]
    public async Task SetDefaultClaims(string uid) => await authService.SetDefaultCustomClaimsAsync(uid);
    
    [HttpPost("{uid}/clear-claims")]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task ClearClaims(string uid) => await authService.ClearCustomClaimsAsync(uid);
    
    [HttpPost("{uid}/request-access")]
    public async Task RequestAccess(string uid) => await authService.RequestAccessAsync(uid);
    
    [HttpPost("{uid}/grant-access")]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task GrantAccess(string uid, [FromBody] GrantAccessRequest body) =>
        await authService.GrantAccessAsync(uid, body.GrantAccess);
    
    [HttpPost("{uid}/revoke-access")]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task RevokeAccess(string uid, [FromBody] RevokeAccessRequest body) =>
        await authService.RevokeAccessAsync(uid, body.NotifyUser);
    
    [HttpPut("{uid}/update-roles")]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task UpdateRoles(string uid, [FromBody] UpdateRolesRequest value) =>
        await authService.UpdateRolesAsync(uid, value);
}