using Mapidemic.Pages.Settings;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

namespace Mapidemic.Pages.Landing;

/// <summary>
/// The HomePage UI for the App
/// </summary>
public partial class HomePage : FlyoutPage
{
	private bool settingsOpen = false; // Not a property as it is used as an out argument

	/// <summary>
	/// The default constructor for the HomePage
	/// </summary>
	public HomePage()
	{
		InitializeComponent();
	}

	/// <summary>
    /// A utility function that all other pages use to render toast notifications
    /// </summary>
    /// <param name="message">message to display in the toast</param>
    public static async Task ShowPopup(string message)
    {
		await Toast.Make(message, ToastDuration.Short, 18).Show();
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
	/// Function that loads the settings page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void SettingsButton_Clicked(object sender, EventArgs args)
	{
		if (ViewPort.Navigation.NavigationStack.LastOrDefault() is not SettingsPage) // only loading settings page if not already loaded
        {
			await ViewPort.PushAsync(new SettingsPage());
        }
	}

	/// <summary>
	/// Function that updates settingsOpen to false.
	/// </summary>
	public void NotInSettings()
	{
		settingsOpen = false;
	}
}