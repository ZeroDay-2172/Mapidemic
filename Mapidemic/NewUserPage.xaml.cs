namespace Mapidemic;

/// <summary>
///  suppressing the warning caused by:
/// [Application.Current!.MainPage = new HomePage();]
/// </summary>
#pragma warning disable CS0618

public partial class NewUserPage : ContentPage
{
    private const int waitingSpeed = 1000;
    private const int transitionSpeed = 750;
    
    /// <summary>
    /// The designated constructor for a ThemePage
    /// </summary>
    public NewUserPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded!;
    }

    /// <summary>
    /// An EH function that controls the first
    /// time user experience
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPageLoaded(object sender, EventArgs e)
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
    /// An EH function changes the app theme when
    /// the user toggles the switch
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
        string entryText = PostalCodeEntry.Text;
        if (await MauiProgram.businessLogic!.ValidatePostalCode(entryText))
        {
            EnterButton.IsEnabled = false;
            PostalCodeEntry.IsEnabled = false;
            // discarding the return task -- for testing purposes only
            _ = MauiProgram.businessLogic.SaveSettings(ThemeToggle.IsToggled, int.Parse(entryText));
            Tracking.IsEnabled = true;
            await PostalCode.FadeTo(0, transitionSpeed);
            Grid.Remove(PostalCode);
            await Tracking.FadeTo(1, transitionSpeed);
            await Task.Delay(waitingSpeed);
            await Tracking.FadeTo(0, transitionSpeed);
            Application.Current!.MainPage = new HomePage();
        }
        else
        {
            await DisplayAlert("Invalid Postal/Zip Code", "We were not able to locate the entered postal/zip code", "OK");
        }
    }
}