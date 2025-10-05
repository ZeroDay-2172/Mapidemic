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
}