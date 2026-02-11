namespace QrSortable;

public partial class MainPage : ContentPage
{
	
	public MainPage()
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
	    Application.Current.Quit();
	    return base.OnBackButtonPressed();
	}
}

