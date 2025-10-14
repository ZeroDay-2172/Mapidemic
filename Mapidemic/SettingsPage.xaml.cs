//Author(s): Connor McGuire

using System.Threading.Tasks;
using Mapidemic.Models;

namespace Mapidemic;

public partial class SettingsPage : ContentPage
{
    private bool darkMode;

    public SettingsPage()
    {
        InitializeComponent();

        //Display current zip code
        ZipCodeEnt.Text = MauiProgram.businessLogic.ReadSettings().PostalCode.ToString();

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
        bool zipValid = await MauiProgram.businessLogic.ValidatePostalCode(ZipCodeEnt.Text);
        if (zipValid)
        {
            bool saveSuccess = await MauiProgram.businessLogic.SaveSettings(darkMode, int.Parse(ZipCodeEnt.Text));

            if (saveSuccess)
            {
                SetDarkMode();
                MauiProgram.businessLogic.ReadSettings().PostalCode = int.Parse(ZipCodeEnt.Text);
                await DisplayAlert("Success!", "Settings have been saved!", "OK");
            }
            else
                await DisplayAlert("Error", "Could not save settings...", "OK");
        }
        else
            await DisplayAlert("Error", "Please enter valid zip code", "OK");
    }

    /// <summary>
    /// Function that sets Dark Mode if darkMode equals true, otherwise sets Light Mode.
    /// </summary>
    /// <param name="dark"></param>
    private void SetDarkMode()
    {
        if (this.darkMode == true)
            Application.Current!.UserAppTheme = AppTheme.Dark;
        else
            Application.Current!.UserAppTheme = AppTheme.Light;
    }
}