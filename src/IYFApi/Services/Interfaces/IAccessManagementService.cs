namespace IYFApi.Services.Interfaces;

public interface IAccessManagementService
{
    public object /*todo: response class*/ RequestAccess(string uid);
    public object GrantAccess(string uid);
}