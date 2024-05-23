using FirebaseAdmin.Auth;
using IYFApi.Models;
using IYFApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IYFApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminAuthorizationFilterAttribute(AdminRole role = AdminRole.Admin) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers
            .TryGetValue("Authorization", out var bearer);
        if (!authHeader || bearer == StringValues.Empty)
        {
            context.Result = UnauthorizedResult("No authorization header provided");
            return;
        }

        var token = bearer.ToString().Split(" ")[1];

        FirebaseToken decodedToken;
        try
        {
            decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        }
        catch (FirebaseAuthException)
        {
            context.Result = UnauthorizedResult("Failed to verify token");
            return;
        }
        
        var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(decodedToken.Uid);
        context.HttpContext.Items["User"] = new UserRecordFix(userRecord);

        var authorized = (bool)decodedToken.Claims[GetRoleString(role)];
        if (!authorized) context.Result = UnauthorizedResult($"Missing required role: {role}");
    }
    
    private static string GetRoleString(AdminRole role) => role switch
    {
        AdminRole.Admin => "admin",
        AdminRole.ContentManager => "contentManager",
        AdminRole.GuestManager => "guestManager",
        AdminRole.AccessManager => "accessManager",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid role")
    };

    private static IActionResult UnauthorizedResult(string? extra = null) => new UnauthorizedObjectResult(
        new ErrorResponse
        {
            Message = "You are not authorized to access this resource"
                      + (extra != null ? " - " + extra : "") + ".",
            Error = "Unauthorized"
        });
}