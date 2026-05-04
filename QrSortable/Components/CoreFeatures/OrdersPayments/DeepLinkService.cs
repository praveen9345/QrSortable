namespace QrSortable.Components.CoreFeatures.OrdersPayments
{
    using QrSortable.Components.CoreFeatures.OrdersPayments.Constants;
    using QrSortable.Components.CoreFeatures.OrdersPayments.ViewModels;

    public class DeepLinkService : IDeepLinkService
    {
        public async Task HandleAsync(Uri uri)
        {
            if (uri == null) return;

            switch (uri.Host.ToLower())
            {
                case "payment-return":
                    await HandlePaymentReturn(uri);
                    break;
            }
        }

        private async Task HandlePaymentReturn(Uri uri)
        {
            var deeplinkId = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("id");
            if (string.IsNullOrEmpty(deeplinkId)) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (deeplinkId == PaymentConstants.PaymentShipmentVmDeeplinkId)
                {
                    var vm = GetCurrentViewModel<PaymentShipmentViewModel>();
                    if (vm != null)
                        await vm.HandleMollieRedirect();
                }
                else if (deeplinkId == PaymentConstants.SubscriptionVmDeeplinkId)
                {
                    var vm = GetCurrentViewModel<SubscriptionViewModel>();
                    if (vm != null)
                        await vm.HandleMollieRedirect();                 
                }
            });
        }

        // Walks the Shell navigation stack from top to bottom looking for a page
        // whose BindingContext matches T. Returns null if not found.
        private static T GetCurrentViewModel<T>() where T : class
        {
            // Check the current visible page first (fastest path).
            if (Shell.Current?.CurrentPage?.BindingContext is T vm)
                return vm;

            // Walk the full navigation stack in case the page is beneath a modal.
            var stack = Shell.Current?.Navigation?.NavigationStack;
            if (stack != null)
            {
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    if (stack[i]?.BindingContext is T found)
                        return found;
                }
            }

            return null;
        }
    }
}