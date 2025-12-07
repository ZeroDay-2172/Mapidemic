using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.FrontMatter;

/// <summary>
/// Suppressing the warning caused by lines Application assignment statements.
/// These statements are obsolete, but still valid 
/// </summary>
#pragma warning disable CS0618

/// <summary>
/// A class that provides a user interface for the Mapidemic logo
/// </summary>
public partial class LogoPage : ContentPage
{
    /// <summary>
    /// The default constructor for a LogoPage
    /// </summary>
    public LogoPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that determines where the user will be directed as the app loads
    /// </summary>
    protected async override void OnAppearing()
    {
        await Logo.FadeTo(1, 1500); // loading in the logo
        if (await MauiProgram.businessLogic.TestDatabaseConnection()) // testing the database connection
        {
            if (MauiProgram.businessLogic.ReadSettings() != null) // checking for first time users
            {
                await Logo.FadeTo(0, 500);
                Application.Current!.MainPage = new HomePage();
            }
            else // having first time users go through setup
            {
                MauiProgram.businessLogic.CreateSettingsFile();
                await Logo.FadeTo(0, 500);
                Application.Current!.MainPage = new NewUserPage();
            }
        }
        else // taking user to an error page if data cannot be accessed from database
        {
            await Logo.FadeTo(0, 500);
            Application.Current!.MainPage = new ReconnectPage();
        }
    }
}