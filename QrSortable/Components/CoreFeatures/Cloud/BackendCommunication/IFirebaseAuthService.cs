namespace QrSortable.Components.CoreFeatures.Cloud.BackendCommunication
{
    public interface IFirebaseAuthService
    {
        Task<string> SignInAnonymouslyAsync();
    }
}
