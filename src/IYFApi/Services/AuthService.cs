using System.Reflection;
using FirebaseAdmin.Auth;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Services.Interfaces;
using MimeKit;
using UserRecord = FirebaseAdmin.Auth.UserRecord;

namespace IYFApi.Services;

public class AuthService(IMailService mailService) : IAuthService
{
    public async Task<IEnumerable<UserRecordFix>> GetAllUsersAsync()
    {
        var userRecords = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
        var users = new List<UserRecord>();
        await foreach (var userRecord in userRecords) users.Add(userRecord);
        return UserRecordFix.FromIEnumerable(users);
    }

    public async Task<UserRecordFix> GetUserAsync(string uid) => await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

    public async Task SetDefaultCustomClaimsAsync(string uid)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        if (user.CustomClaims.Count > 0)
            throw new InvalidOperationException("User already has custom claims");

        var isAdminByDefault = user.Email != null && user.Email.EndsWith("@iyf.hu") && user.EmailVerified;
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, new UserRoleClaims
        {
            AccessRequested = false,
            AccessDenied = false,
            Admin = isAdminByDefault,
            ContentManager = false,
            GuestManager = false,
            AccessManager = false
        }.ToObjDictionary());
    }

    public async Task ClearCustomClaimsAsync(string uid) =>
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, new Dictionary<string, object>());

    public async Task RequestAccessAsync(string uid)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

        if (user.CustomClaims.TryGetValue("accessRequested", out var accessRequested) && (bool)accessRequested)
            throw new InvalidOperationException("Access request has been already sent");

        if (user.CustomClaims.TryGetValue("accessDenied", out var accessDenied) && (bool)accessDenied)
        {
            await GrantAccessAsync(uid, false);
            return;
        }

        if (user.CustomClaims.TryGetValue("admin", out var admin) && (bool)admin)
            throw new InvalidOperationException("The requested user was already given access");

        await SetCustomUserClaimsKeepExisting(user, new UserRoleClaims { AccessRequested = true },
            UserRoleClaims.OverrideMode.Override);

        var managers = (await GetAllUsersAsync())
            .Where(u =>
                u.CustomClaims.TryGetValue("accessManager", out var accessManager) && (bool)accessManager)
            .Select(u => new MailboxAddress(u.DisplayName, u.Email)).ToList();
        if (managers.Count == 0) return;

        var template = LoadEmailTemplate(ResourceNames.AccessRequest, new Dictionary<string, string>
        {
            { "name", user.DisplayName },
            { "email", user.Email },
            { "photoUrl", user.PhotoUrl }
        });
        mailService.SendEmail(managers, "Access request", template);
    }

    public async Task GrantAccessAsync(string uid, bool grant)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

        if (user.CustomClaims.TryGetValue("admin", out var admin) && (bool)admin)
            throw new InvalidOperationException("The requested user was already given access");

        await SetCustomUserClaimsKeepExisting(user,
            new UserRoleClaims { AccessRequested = false, AccessDenied = !grant, Admin = grant },
            UserRoleClaims.OverrideMode.Override);

        if (!user.EmailVerified) return;

        var recipient = new MailboxAddress(user.DisplayName, user.Email);
        var template = LoadEmailTemplate(grant ? ResourceNames.AccessGranted : ResourceNames.AccessDenied,
            new Dictionary<string, string> { { "name", user.DisplayName } });
        mailService.SendEmail([recipient], "Access granted", template);
    }

    public async Task RevokeAccessAsync(string uid, bool notifyUser)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

        if (!user.CustomClaims.TryGetValue("admin", out var admin) || !(bool)admin)
            throw new InvalidOperationException("The requested user doesn't have admin access");

        await SetCustomUserClaimsKeepExisting(user, new UserRoleClaims { Admin = false },
            UserRoleClaims.OverrideMode.Override);

        if (!notifyUser || !user.EmailVerified) return;

        var recipient = new MailboxAddress(user.DisplayName, user.Email);
        var template = LoadEmailTemplate(ResourceNames.AccessRevoked,
            new Dictionary<string, string> { { "name", user.DisplayName } });
        mailService.SendEmail([recipient], "Access revoked", template);
    }

    public async Task UpdateRolesAsync(string uid, UpdateRolesRequest value)
    {
        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        await SetCustomUserClaimsKeepExisting(user, new UserRoleClaims
        {
            ContentManager = value.ContentManager,
            GuestManager = value.GuestManager,
            AccessManager = value.AccessManager
        }, UserRoleClaims.OverrideMode.Override);

        var rolesUpdated = new Dictionary<AdminRole, bool>();
        if (value.ContentManager.HasValue) rolesUpdated.Add(AdminRole.ContentManager, value.ContentManager.Value);
        if (value.GuestManager.HasValue) rolesUpdated.Add(AdminRole.GuestManager, value.GuestManager.Value);
        if (value.AccessManager.HasValue) rolesUpdated.Add(AdminRole.AccessManager, value.AccessManager.Value);

        var recipient = new MailboxAddress(user.DisplayName, user.Email);
        var template = LoadRolesUpdatedEmailTemplate(user.DisplayName, rolesUpdated);
        mailService.SendEmail([recipient], "Roles updated", template);
    }

    private static async Task SetCustomUserClaimsKeepExisting(UserRecord user, Dictionary<string, bool?> newClaims,
        UserRoleClaims.OverrideMode overrideMode)
    {
        var existingClaims = user.CustomClaims?.ToDictionary() ?? new Dictionary<string, object>();
        var keys = existingClaims.Keys.Union(newClaims.Keys);
        var mergedClaims = keys.ToDictionary(key => key,
            key => (object)GetClaimValueByMode((
                existingClaims.TryGetValue(key, out var val1) ? (bool?)val1 : null,
                newClaims.GetValueOrDefault(key)
            ), overrideMode));

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid, mergedClaims);
    }

    private static bool GetClaimValueByMode(
        (bool? existingValue, bool? newValue) values,
        UserRoleClaims.OverrideMode mode) =>
        mode switch
        {
            UserRoleClaims.OverrideMode.KeepExisting => (values.existingValue ?? values.newValue)!.Value,
            UserRoleClaims.OverrideMode.Override => (values.newValue ?? values.existingValue)!.Value,
            UserRoleClaims.OverrideMode.KeepTrue => (values.existingValue ?? false) || (values.newValue ?? false),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    private static string LoadEmailTemplate(string resourceName, Dictionary<string, string>? templateValues = null)
    {
        var assembly = Assembly.GetExecutingAssembly();
        try
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            var template = reader.ReadToEnd();
            if (templateValues == null) return template;

            return templateValues.Aggregate(template,
                (current, pair) => current.Replace($"{{{{{pair.Key}}}}}", pair.Value));
        }
        catch (Exception e)
        {
            throw new Exception("Failed to load email template", e);
        }
    }

    private static string LoadRolesUpdatedEmailTemplate(string name, Dictionary<AdminRole, bool> roleUpdates)
    {
        var template = LoadEmailTemplate(ResourceNames.RolesUpdated);

        var listItems = roleUpdates.Select(pair =>
        {
            var roleDisplayName = pair.Key switch
            {
                AdminRole.ContentManager => "Content Manager",
                AdminRole.GuestManager => "Guest Manager",
                AdminRole.AccessManager => "Access Manager",
                _ => throw new ArgumentOutOfRangeException()
            };
            return $"<li>You were {(pair.Value ? "granted" : "revoked")} the {roleDisplayName} role</li>\n";
        });

        return template
            .Replace("{{name}}", name)
            .Replace("{{roleUpdates}}", string.Join("", listItems.ToArray()));
    }

    private class UserRoleClaims
    {
        public bool? AccessRequested { get; init; }
        public bool? AccessDenied { get; init; }
        public bool? Admin { get; init; }
        public bool? ContentManager { get; init; }
        public bool? GuestManager { get; init; }
        public bool? AccessManager { get; init; }

        public static implicit operator Dictionary<string, bool?>(UserRoleClaims claims) =>
            new()
            {
                { "accessRequested", claims.AccessRequested },
                { "accessDenied", claims.AccessDenied },
                { "admin", claims.Admin },
                { "contentManager", claims.ContentManager },
                { "guestManager", claims.GuestManager },
                { "accessManager", claims.AccessManager }
            };

        public Dictionary<string, object?> ToObjDictionary() => ((Dictionary<string, bool?>)this)
            .ToDictionary(pair => pair.Key, pair => (object?)pair.Value);

        public enum OverrideMode
        {
            KeepExisting,
            Override,
            KeepTrue
        }
    }

    private static class ResourceNames
    {
        public const string AccessRequest = "IYFApi.Resources.AccessRequestEmailTemplate.html";
        public const string AccessGranted = "IYFApi.Resources.AccessGrantedEmailTemplate.html";
        public const string AccessDenied = "IYFApi.Resources.AccessDeniedEmailTemplate.html";
        public const string AccessRevoked = "IYFApi.Resources.AccessRevokedEmailTemplate.html";
        public const string RolesUpdated = "IYFApi.Resources.RolesUpdatedEmailTemplate.html";
    }
}