namespace Mapidemic;

/// <summary>
/// The HomePage UI for the App
/// </summary>
public partial class HomePage : FlyoutPage
{
	/// <summary>
	/// The default constructor for the HomePage
	/// </summary>
	public HomePage()
	{
		InitializeComponent();
	}

	/// <summary>
	/// A function that returns the viewport of
	/// the navigation page inside the home page
	/// </summary>
	/// <returns>The contained viewport</returns>
	public NavigationPage GetViewport()
	{
		return ViewPort;
	}

	/// <summary>
	/// A function that loads the symptom checker
	/// feature of the app
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void OnClickedSymptoms(object sender, EventArgs args)
	{
		NavigationPage symptomsPage = new NavigationPage(new SymptomsPage());
		
		symptomsPage.BarTextColor = Color.FromArgb("#FFFFFF");
		symptomsPage.BarBackgroundColor = Color.FromArgb("#0074C0");
		await ViewPort.PushAsync(symptomsPage);
		IsPresented = false;
	}
}