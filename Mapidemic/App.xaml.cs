using Mapidemic.Models;

namespace Mapidemic;

/// <summary>
/// A class that runs the Mapidemic app on startup
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// The designated constructor for an App
	/// </summary>
	public App()
	{
		InitializeComponent();
	}

	/// <summary>
	/// A function that starts the app from the
	/// perspective of a new user, or an existing
	/// user based on if settings have been set up
	/// </summary>
	/// <param name="activationState"></param>
	/// <returns>The app window</returns>
	protected override Window CreateWindow(IActivationState? activationState)
	{
		Settings settings = MauiProgram.businessLogic!.ReadSettings();
		if (settings != null) // loading app theme if existing user
		{
			Current!.UserAppTheme = settings.Theme;
		}
		return new Window(new AppShell());
	}
}