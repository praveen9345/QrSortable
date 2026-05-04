namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    public interface IDeepLinkService
    {
        Task HandleAsync(Uri uri);
    }
}
