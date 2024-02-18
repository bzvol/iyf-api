using Microsoft.AspNetCore.Mvc;

namespace IYFApi;

public static class Utils
{
    public static string GetUid(this ControllerBase controller) => // Get uid from authorization header
        controller.HttpContext.Request.Headers.Authorization.ToString().Split(' ')[1] ??
        throw new Exception("No authorization header found");
}