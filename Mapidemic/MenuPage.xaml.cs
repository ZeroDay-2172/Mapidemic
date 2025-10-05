namespace Mapidemic;

public partial class MenuPage : ContentPage
{
    private NavigationPage? viewport;
	private const string cdcLink = "https://www.cdc.gov/";

    /// <summary>
    /// The designated constructor for a MenuPage
    /// </summary>
    public MenuPage()
    {
        InitializeComponent();
    }

    /// <summary>
	/// A function that prepares the viewport after
	/// it has been loaded with a ContentPage
	/// </summary>
	private async void PrepareViewport()
	{
		viewport!.BarTextColor = Color.FromArgb("#FFFFFF");
		viewport.BarBackgroundColor = Color.FromArgb("#0074C0");
		HomePage? homePage = this.Parent as HomePage;
		await homePage!.GetViewport().PushAsync(viewport);
		homePage.IsPresented = false;
	}

	/// <summary>
	/// A function that displays the about us page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public void AuButton_Clicked(object sender, EventArgs args)
	{
		viewport = new NavigationPage(new AboutUs());
		PrepareViewport();
	}

	/// <summary>
	/// A function that displays the who we are page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public void PpButton_Clicked(object sender, EventArgs args)
	{
		viewport = new NavigationPage(new PrivacyPolicyPage());
		PrepareViewport();
	}

	/// <summary>
	/// A function that displays the contact information page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public void CiButton_Clicked(object sender, EventArgs args)
	{
		viewport = new NavigationPage(new ContactInformation());
		PrepareViewport();
	}

	/// <summary>
	/// A function that directs the user to the
	/// website of the CDC
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void CdcButton_Clicked(object sender, EventArgs args)
	{
		await Launcher.Default.OpenAsync(new Uri(cdcLink));
	}
}