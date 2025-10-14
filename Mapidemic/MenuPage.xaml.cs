using Mapidemic.Models;
using System.Runtime.CompilerServices;

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
	/// A function that displays the symptoms page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void ScButton_Clicked(object sender, EventArgs args)
	{
		viewport = new NavigationPage(new SymptomsPage());
		PrepareViewport();
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
	/// A function that displays the settings page
	/// Note: Needs to be public for settingsPage to work!
	/// </summary>
	/// <param name="openSettings">If settings should be opened</param>
	/// <param></param>
	public async void SettingsPageHandler() // TODO: Hide settings icon when already on settings page
	{
		var homePage = this.Parent as HomePage;
		if (homePage == null) // If not in a HomePage
		{
			return;
		}
		var nav = homePage.GetViewport(); // NavigationPage

		var stack = nav.Navigation?.NavigationStack;
		if (stack != null && stack.Count > 0 && stack[stack.Count - 1] is SettingsPage) // Already on settings page
		{
			return;
		}
		await nav.PushAsync(new SettingsPage()); // Push settings page
		homePage.IsPresented = false; // Close the menu
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