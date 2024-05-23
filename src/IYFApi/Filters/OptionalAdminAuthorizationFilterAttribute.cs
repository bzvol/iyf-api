using FirebaseAdmin.Auth;
using IYFApi.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IYFApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class OptionalAdminAuthorizationFilterAttribute(AdminRole role = AdminRole.Admin) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Items["IsAuthorized"] = false;
        
        var authHeader = context.HttpContext.Request.Headers
            .TryGetValue("Authorization", out var bearer);
        if (!authHeader || bearer == StringValues.Empty)
        {
            await next();
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
            await next();
            return;
        }
        
        var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(decodedToken.Uid);
        context.HttpContext.Items["User"] = new UserRecordFix(userRecord);
        
        var authorized = (bool)decodedToken.Claims[GetRoleString(role)];
        context.HttpContext.Items["IsAuthorized"] = authorized;
        
        await next();
    }

    private static string GetRoleString(AdminRole role) => role switch
    {
        AdminRole.Admin => "admin",
        AdminRole.ContentManager => "contentManager",
        AdminRole.GuestManager => "guestManager",
        AdminRole.AccessManager => "accessManager",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid role")
    };
}