using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.FrontMatter;

/// <summary>
///  suppressing the warning caused by lines: 
/// </summary>
#pragma warning disable CS0618

public partial class LogoPage : ContentPage
{
    /// <summary>
    /// The designated constructor for a LogoPage
    /// </summary>
    public LogoPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded!;
    }

    /// <summary>
    /// A function that determines where the user will be
    /// directed as the app loads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnPageLoaded(object sender, EventArgs e)
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