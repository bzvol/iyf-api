using FirebaseAdmin.Auth;
using IYFApi.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IYFApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
{
    private readonly AdminRole _role;

    public AdminAuthorizationFilterAttribute(AdminRole role)
    {
        _role = role;
    }

    public async void OnAuthorization(AuthorizationFilterContext context)
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

        var authorized = (bool)decodedToken.Claims[GetRoleString(_role)];
        if (!authorized) context.Result = UnauthorizedResult($"Missing required role: {_role}");
    }

#pragma warning disable CS8524
    private static string GetRoleString(AdminRole role) => role switch
    {
        AdminRole.ContentManager => "content_manager",
        AdminRole.GuestManager => "guest_manager",
        AdminRole.AccessManager => "access_manager"
    };
#pragma warning restore CS8524

    private static IActionResult UnauthorizedResult(string? extra = null) => new UnauthorizedObjectResult(
        new ErrorResponse
        {
            Message = "You are not authorized to access this resource"
                      + (extra != null ? " - " + extra : "") + ".",
            Error = "Unauthorized"
        });
}