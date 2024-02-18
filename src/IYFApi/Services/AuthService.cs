using FirebaseAdmin.Auth;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace IYFApi.Services;

public class AuthService : IAuthService
{
    public async Task<IEnumerable<UserRecord>> GetAllUsers()
    {
        var userRecords = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
        var users = new List<UserRecord>();
        await foreach (var userRecord in userRecords) users.Add(userRecord);
        return users;
    }

    public async Task SetDefaults(string uid)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        var isAdminByDefault = user.Email != null && user.Email.EndsWith("@iyf.hu") && user.EmailVerified;
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, new Dictionary<string, object>
        {
            { "accessRequested", false },
            { "admin", isAdminByDefault },
            { "contentManager", false },
            { "guestManager", false },
            { "accessManager", false }
        });
    }

    public async Task RequestAccess(string uid)
    {
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, new Dictionary<string, object>
        {
            { "accessRequested", true }
        });
        
        var managers = (await GetAllUsers())
            .Where(u => 
                u.CustomClaims.TryGetValue("accessManager", out var accessManager) && (bool)accessManager)
            .Select(u => u.Email);
        // TODO: Send emails
    }

    public async Task GrantAccess(string uid)
    {
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, new Dictionary<string, object>
        {
            { "accessRequested", false },
            { "admin", true }
        });
    }

    public static async Task<string> GetUidFromRequest(HttpRequest request)
    {
        var authHeader = request.Headers.TryGetValue("Authorization", out var bearer);
        if (!authHeader || bearer == StringValues.Empty)
            throw new Exception("No authorization header found");

        var token = bearer.ToString().Split(" ")[1];
        var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        return decodedToken.Uid;
    }
}