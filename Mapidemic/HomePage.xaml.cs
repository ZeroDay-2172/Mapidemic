using System.Threading.Tasks;
using Mapidemic.Models;

namespace Mapidemic;

/// <summary>
/// The HomePage UI for the App
/// </summary>
public partial class HomePage : FlyoutPage
{
	private NavigationPage? viewport;

	//Not a property as it is used as an out argument
	private bool settingsOpen = false;

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
	/// Function that loads the settings page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public void SettingsButton_Clicked(object sender, EventArgs args)
	{
		if (settingsOpen == false)
			this.menuPage.SettingsPageHandler(out settingsOpen);
	}

	/// <summary>
	/// Function that updates settingsOpen to false.
	/// </summary>
	public void NotInSettings()
	{
		settingsOpen = false;
	}
}