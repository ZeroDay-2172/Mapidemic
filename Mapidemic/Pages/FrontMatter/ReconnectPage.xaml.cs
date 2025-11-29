namespace Mapidemic.Pages.FrontMatter;

/// <summary>
///  suppressing the warning caused by:
/// [Application.Current!.MainPage = new HomePage();]
/// </summary>
#pragma warning disable CS0618

public partial class ReconnectPage : ContentPage
{
    /// <summary>
    /// The designated constructor for a ReconnectPage
    /// </summary>
    public ReconnectPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that checks if the database connection is working
    /// and if so, lets the user into the app
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void TryAgain_Clicked(Object sender, EventArgs args)
    {
        Popup.IsOpen = true;
		await Task.Yield();
        if (await MauiProgram.businessLogic.TestDatabaseConnection()) // testing the database connection
        {
            if (MauiProgram.businessLogic.ReadSettings() != null) // checking for first time users
            {
                await this.FadeTo(0, 500);
                Application.Current!.MainPage = new HomePage();
            }
            else // having first time users go through setup
            {
                MauiProgram.businessLogic.CreateSettingsFile();
                await this.FadeTo(0, 500);
                Application.Current!.MainPage = new NewUserPage();
            }
        }
        else // alerting user when app cannot read data from database
        {
            await DisplayAlert("Connection Error", "Connection attempt failed", "OK");
        }
        Popup.IsOpen = false;
    }
}