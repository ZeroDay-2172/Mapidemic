using Mapidemic.Models;
using System.Text.Json;

namespace Mapidemic;

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
		Settings settings = MauiProgram.businessLogic.ReadSettings();
		if (settings != null)
		{
			Current!.UserAppTheme = settings.Theme;
			return new Window(new HomePage());
		}
		else
		{
			MauiProgram.businessLogic.CreateSettingsFile();
			return new Window(new AppShell());
		}
	}
}