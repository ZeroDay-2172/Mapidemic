using System.Diagnostics;
using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.FrontMatter;

/// <summary>
/// Suppressing the warning caused by lines Application assignment statements.
/// These statements are obsolete, but still valid 
/// </summary>
#pragma warning disable CS0618

/// <summary>
/// A class that provides a user interface for a new user to configure their app settings
/// </summary>
public partial class NewUserPage : ContentPage
{
    private const int waitingSpeed = 1000;
    private const int transitionSpeed = 750;
    
    /// <summary>
    /// The default constructor for a ThemePage
    /// </summary>
    public NewUserPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// An EH function that controls the first time user experience
    /// </summary>
    protected async override void OnAppearing()
    {
        Welcome.IsEnabled = true;
        await Welcome.FadeTo(1, transitionSpeed);
        await Task.Delay(waitingSpeed);
        await Welcome.FadeTo(0, transitionSpeed);
        Setup.IsEnabled = true;
        Grid.Remove(Welcome);
        await Setup.FadeTo(1, transitionSpeed);
        await Task.Delay(waitingSpeed);
        await Setup.FadeTo(0, transitionSpeed);
        Theme.IsEnabled = true;
        Grid.Remove(Setup);
        // ensuring the switch matches the user's theme
        switch (Application.Current?.RequestedTheme)
        {
            case AppTheme.Light: ThemeToggle.IsToggled = false; break;
            case AppTheme.Dark: ThemeToggle.IsToggled = true; break;
            default: break;
        }
        // attaching an EH to act when the switch is toggled
        ThemeToggle.Toggled += OnThemeToggled!;
        await Theme.FadeTo(1, transitionSpeed);
    }

    /// <summary>
    /// An EH function changes the app theme when the user toggles the switch
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnThemeToggled(object sender, EventArgs e)
    {
        Application.Current!.UserAppTheme = ThemeToggle.IsToggled ? AppTheme.Dark : AppTheme.Light;
    }

    /// <summary>
    /// An EH function that takes the user's choice
    /// from the switch and passes it to the ThemePage
    /// along with the choice from the unit page after fading out
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnChoiceSelected(object sender, EventArgs e)
    {
        ThemeToggle.IsEnabled = false;
        ContinueButton.IsEnabled = false;
        await Theme.FadeTo(0, transitionSpeed);
        PostalCode.IsEnabled = true;
        Grid.Remove(Theme);
        // setting the Entry placeholder to white for visability
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            PostalCodeEntry.PlaceholderColor = Colors.White;
        }
        await PostalCode.FadeTo(1, transitionSpeed);
    }

    /// <summary>
    /// An EH function that takes the user's postal code
    /// and passes it, and the unit and theme information from
    /// the previous pages, to the businessLogic to be validated
    /// and written to the local settings file before fading out
    /// to the next page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnEnterClicked(object sender, EventArgs e)
    {
        Popup.IsOpen = true;
        string entryText = PostalCodeEntry.Text;
        try
        {
            if (await MauiProgram.businessLogic!.ValidatePostalCode(entryText)) // attempting to validate postal code
            {
                if (await MauiProgram.businessLogic.SaveSettings(ThemeToggle.IsToggled, int.Parse(entryText))) // attempting to parse settings to JSON
                {
                    Popup.IsOpen = false;
                    Tracking.IsEnabled = true;
                    await PostalCode.FadeTo(0, transitionSpeed);
                    Grid.Remove(PostalCode);
                    await Tracking.FadeTo(1, transitionSpeed);
                    await Task.Delay(waitingSpeed);
                    await Tracking.FadeTo(0, transitionSpeed);
                    Application.Current!.MainPage = new HomePage();
                }
                else // error if JSON parse failed
                {
                    Popup.IsOpen = false;
                    await HomePage.ShowPopup("Unable to save to settings. Please try again");
                }
            }
            else // error if the postal code entered was invalid
            {
                Popup.IsOpen = false;
                await HomePage.ShowPopup("Unable to validate postal code. Please try again");
            }
        }
        catch(Exception error) // error if the user lost connection during this step
        {
            Popup.IsOpen = false;
            await HomePage.ShowPopup("Unable to validate postal code. Please try again");
            Debug.WriteLine(error.Message);
        }
    }
}