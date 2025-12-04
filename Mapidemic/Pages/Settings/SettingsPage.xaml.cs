using System.Diagnostics;
using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.Settings;

/// <summary>
/// A class that provides a user interface for the user to change their settings
/// </summary>
public partial class SettingsPage : ContentPage
{
    private bool darkMode;

    /// <summary>
    /// The designated constructor for the SettingsPage
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();

        string postalCode = MauiProgram.businessLogic!.ReadSettings().PostalCode.ToString(); // Displaying current postal code
        switch(postalCode.Length)
        {
            case 3: PostalCodeEnt.Text = $"00{postalCode}"; break;
            case 4: PostalCodeEnt.Text = $"0{postalCode}"; break;
            default: PostalCodeEnt.Text = postalCode; break;
        }
        
        if (Application.Current?.RequestedTheme == AppTheme.Dark) // moving switch to true
        {
            DarkModeSwitch.IsToggled = true;
        }
        else // moving switch to false
        {
            DarkModeSwitch.IsToggled = false;
        }
    }

    /// <summary>
    /// Function that sets dark mode to be toggled on the next settings save.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void OnDarkModeToggled(Object sender, EventArgs args)
    {
        if (DarkModeSwitch.IsToggled) // storing true position of the switch
        {
            darkMode = true;
        }
        else // storing false position of the switch
        {
            darkMode = false;
        }
    }

    /// <summary>
    /// Function that updates the user's settings.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void SaveButton_Clicked(Object sender, EventArgs args)
    {
        try // attempting to save the settings
        {
            if (await MauiProgram.businessLogic.ValidatePostalCode(PostalCodeEnt.Text)) // validating postal code
            {
                if (await MauiProgram.businessLogic.SaveSettings(darkMode, int.Parse(PostalCodeEnt.Text))) // saving settings
                {
                    if (darkMode) // updating app theme to dark theme
                    {
                        Application.Current!.UserAppTheme = AppTheme.Dark;
                    }
                    else // updating app theme to light theme
                    {
                        Application.Current!.UserAppTheme = AppTheme.Light;
                    }
                    MauiProgram.businessLogic.ReadSettings().PostalCode = int.Parse(PostalCodeEnt.Text);
                    await HomePage.ShowPopup("Settings have been saved");
                }
                else // toast if settings could not be saved
                {
                    await HomePage.ShowPopup("Settings could not be saved");
                }
            }
            else // invalid postal code
            {
                await HomePage.ShowPopup("Invalid postal code entered");
            }
        }
        catch (Exception error) // error if unable to save to local settings file
        {
            await HomePage.ShowPopup("Settings could not be saved");
            Debug.WriteLine(error.Message);
        }
    }
}