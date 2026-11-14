namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using QrSortable.Components.CoreFeatures.Scanner.Models;
    using QrSortable.Components.UiFunctionality.Navigation.Views;
    using ViewModels;

    /// <summary>
    /// The code behind of the BoxDetail view.
    /// </summary>
    public partial class BoxDetailView : BaseView
    {
        /// <summary>
        ///  Initializes a new instance of the BoxDetail class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The BoxDetailModel associated with this view.</param>
        public BoxDetailView(BoxDetailViewModel viewModel):base(viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BoxDetailViewModel vm) vm.FocusRequested += OnFocusRequested;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is BoxDetailViewModel vm) vm.FocusRequested -= OnFocusRequested;
        }

        private void OnFocusRequested(SuggestionField field) =>
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (field == SuggestionField.Location) locationEntry.Focus();
                else categoryEntry.Focus();
            });

        private void OnLocationFocused(object sender, FocusEventArgs e) =>
            (BindingContext as BoxDetailViewModel)?.SetActiveField(SuggestionField.Location);

        private void OnCategoryFocused(object sender, FocusEventArgs e) =>
            (BindingContext as BoxDetailViewModel)?.SetActiveField(SuggestionField.Category);
    }
}