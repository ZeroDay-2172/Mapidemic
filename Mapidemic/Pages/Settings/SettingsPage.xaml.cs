namespace Mapidemic.Pages.Settings;

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
                    await ShowPopup("Success!", "Settings have been saved!");
                }
                else
                {
                    await ShowPopup("Error", "Could not save settings...");
                }
            }
            else
            {
                await ShowPopup("Error", "Please enter a valid zip code!");
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
    /// <param name="title">title for the popup</param>
    /// <param name="message">message to display in the popup</param>
    private async Task ShowPopup(string title, string message)
    {
        settingsPopup.ContentTemplate = new DataTemplate(() =>
        {
            // Create the frame
            Frame popupFrame = new Frame
            {
                CornerRadius = 6,
                Padding = 12,
                Margin = new Thickness(0,0,0,20),
                BackgroundColor = Colors.Transparent,
                HasShadow = false,
                BorderColor = Colors.Transparent
            };

            // Create label to hold the text
            Label popupLabel = new Label
            {
                Text = message,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 18
            };

            popupFrame.Content = popupLabel;
            return popupFrame;
        });

        settingsPopup.HeaderTitle = title;
        settingsPopup.Show();
    }
}