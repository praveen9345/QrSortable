namespace QrSortable.Components.UiFunctionality.Navigation.Views
{
    using QrSortable.Components.CoreFeatures.DataManagement.General.Models;
    using QrSortable.Components.CoreFeatures.Scanner.Views;
    using QrSortable.Components.UiFunctionality.Localization;
    using ViewModels;

    /// <summary>
    /// The code behind of the RootView view.
    /// </summary>
    public partial class RootView : BaseView
    {
        private readonly RootViewModel _viewModel;

        /// <summary>
        ///  Initializes a new instance of the RootView class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The RootViewModel associated with this view.</param>
        public RootView(RootViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            Application.Current.Quit();
            return base.OnBackButtonPressed();
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = (CollectionView)sender;
            var selectedItem = collectionView.SelectedItem;

            if (selectedItem != null)
            {
                try
                {
                    // Handle selection
                    var selected = selectedItem as StorageEntry;
                  
                    if (selected != null)
                    {
                        string displayValue = selected.BarcodeValue + "," + selected.BarcodeType;
                        await _viewModel.NavigationService.Navigate<BoxDetailView>(displayValue);
                    }
                    else
                    {
                        await _viewModel.DialogService.ShowAlertDialog("The selected item could not be found.", AppResources.Dialog_OK_Text);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"RootView.cs.OnSelectionChanged: Exception: {ex}");
                }
            }
        }
    }
}