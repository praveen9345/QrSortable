namespace QrSortable.Components.CoreFeatures.OrdersPayments.Views
{
    using Microsoft.Maui.Controls;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using System.IO;
    using ViewModels;

    /// <summary>
    /// The code behind of the PaymentShipmentView view.
    /// </summary>
    public partial class PaymentShipmentView : BaseView
    {

        /// <summary>
        ///  Initializes a new instance of the PaymentShipmentViewModel class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The PaymentShipmentViewModel associated with this view.</param>
        public PaymentShipmentView(PaymentShipmentViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}