using FirebaseAdmin.Auth;
using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class UsersController(IAuthService authService) : ControllerBase
{
    [HttpGet]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task<IEnumerable<UserRecord>> GetAllUsers() => await authService.GetAllUsers();
    
    [HttpPost("{uid}/set-defaults")]
    public async Task SetDefaults(string uid) => await authService.SetDefaults(uid);
    
    [HttpPost("{uid}/request-access")]
    public async Task RequestAccess(string uid) => await authService.RequestAccess(uid);
    
    [HttpPost("{uid}/grant-access")]
    [AdminAuthorizationFilter(AdminRole.AccessManager)]
    public async Task GrantAccess(string uid) => await authService.GrantAccess(uid);
}