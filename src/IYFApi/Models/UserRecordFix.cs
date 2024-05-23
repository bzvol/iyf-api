using FirebaseAdmin.Auth;

namespace IYFApi.Models;

/**
 * This class is a fix for the UserRecord class.
 * Firebase in its client SDKs uses `photoURL` as the
 * property name for the user's photo URL, while it uses
 * `photoUrl` in every server side SDK. Therefore,
 * it is never (de)serialized correctly.
 */
public class UserRecordFix(UserRecord userRecord)
{
    public string Uid => userRecord.Uid;
    public string Email => userRecord.Email;
    public bool EmailVerified => userRecord.EmailVerified;
    public string DisplayName => userRecord.DisplayName;

    // ReSharper disable once InconsistentNaming
    public string PhotoURL => userRecord.PhotoUrl; // Fix
    public bool Disabled => userRecord.Disabled;
    public string PhoneNumber => userRecord.PhoneNumber;
    public IReadOnlyDictionary<string, object> CustomClaims => userRecord.CustomClaims;
    public string ProviderId => userRecord.ProviderId;
    public IUserInfo[] ProviderData => userRecord.ProviderData;
    public DateTime TokensValidAfterTimestamp => userRecord.TokensValidAfterTimestamp;
    public UserMetadata UserMetaData => userRecord.UserMetaData;
    public string TenantId => userRecord.TenantId;

    public static implicit operator UserRecordFix(UserRecord userRecord) => new(userRecord);

    public static IEnumerable<UserRecordFix> FromIEnumerable(IEnumerable<UserRecord> userRecords) =>
        userRecords.Select(userRecord => new UserRecordFix(userRecord));
}