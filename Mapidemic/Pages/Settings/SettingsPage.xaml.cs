namespace Mapidemic.Pages.Settings;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public partial class SettingsPage : ContentPage
{
    private bool darkMode;

    public SettingsPage()
    {
        InitializeComponent();

        //Display current postal code
        string postalCode = MauiProgram.businessLogic!.ReadSettings().PostalCode.ToString();
        switch(postalCode.Length)
        {
            case 3: PostalCodeEnt.Text = $"00{postalCode}"; break;
            case 4: PostalCodeEnt.Text = $"0{postalCode}"; break;
            default: PostalCodeEnt.Text = postalCode; break;
        }
        

        if (Application.Current?.RequestedTheme == AppTheme.Dark)
            DarkModeSwitch.IsToggled = true;
        else
            DarkModeSwitch.IsToggled = false;
    }

    /// <summary>
    /// Function that sets dark mode to be toggled on the next
    /// settings save.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void OnDarkModeToggled(Object sender, EventArgs args)
    {
        if (DarkModeSwitch.IsToggled == true)
            darkMode = true;
        else
            darkMode = false;
    }

    /// <summary>
    /// Function that updates the user's settings.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void SaveButton_Clicked(Object sender, EventArgs args)
    {
        try
        {
            if (await MauiProgram.businessLogic.ValidatePostalCode(PostalCodeEnt.Text))
            {
                if (await MauiProgram.businessLogic.SaveSettings(darkMode, int.Parse(PostalCodeEnt.Text)))
                {
                    SetDarkMode();
                    MauiProgram.businessLogic.ReadSettings().PostalCode = int.Parse(PostalCodeEnt.Text);
                    await ShowPopup("Settings have been saved!");
                }
                else
                {
                    await ShowPopup("Could not save settings...");
                }
            }
            else
            {
                await ShowPopup("Please enter a valid zip code!");
            }
        }
        catch (Exception error)
        {
            await DisplayAlert("Network Error", $"{error.Message}", "OK");
        }
        finally
        {
            
        }
    }

    /// <summary>
    /// Function that sets Dark Mode if darkMode equals true, otherwise sets Light Mode.
    /// </summary>
    private void SetDarkMode()
    {
        if (this.darkMode == true)
            Application.Current!.UserAppTheme = AppTheme.Dark;
        else
            Application.Current!.UserAppTheme = AppTheme.Light;
    }

    /// <summary>
    /// Method that shows a popup with the provided message
    /// </summary>

    /// <param name="message">message to display in the popup</param>
    private async Task ShowPopup(string message)
    {
        var popup = Toast.Make(
            message, 
            ToastDuration.Short,
            textSize: 18
        );
        await popup.Show();
    }
}