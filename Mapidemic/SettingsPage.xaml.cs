//Author(s): Connor McGuire
namespace Mapidemic;

public partial class SettingsPage : ContentPage
{
    private bool darkMode;

    public SettingsPage()
    {
        InitializeComponent();

        //Display current postal code
        string postalCode = MauiProgram.businessLogic.ReadSettings().PostalCode.ToString();
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
        bool postalValid = await MauiProgram.businessLogic.ValidatePostalCode(PostalCodeEnt.Text);
        if (postalValid)
        {
            bool saveSuccess = await MauiProgram.businessLogic.SaveSettings(darkMode, int.Parse(PostalCodeEnt.Text));

            if (saveSuccess)
            {
                SetDarkMode();
                MauiProgram.businessLogic.ReadSettings().PostalCode = int.Parse(PostalCodeEnt.Text);
                await DisplayAlert("Success!", "Settings have been saved!", "OK");
            }
            else
                await DisplayAlert("Error", "Could not save settings...", "OK");
        }
        else
            await DisplayAlert("Error", "Please enter postal zip code", "OK");
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