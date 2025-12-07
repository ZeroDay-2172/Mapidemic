namespace Mapidemic.Pages.Menu;

/// <summary>
/// A class that provides a user interface for displaying the data collected by the Mapidemic team
/// </summary>
public partial class InformationWeCollectPage : ContentPage
{
    /// <summary>
    /// The default constructor for an InformationWeCollectPage
    /// </summary>
    public InformationWeCollectPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that controls the toggle status of a section panel
    /// </summary>
    /// <param name="panel"></param>
    private void ToggleAsync(View panel)
    {
        if (panel.IsVisible) // closing the panel
        {
            panel.IsVisible = false;
        }
        else // opening the panel
        {
            panel.IsVisible = true;
        }
    }

    /// <summary>
    /// A function that toggles the intro content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnIntro(object sender, EventArgs e) => ToggleAsync(IntroContent);

    /// <summary>
    /// A function that toggles the illness reports content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnIllnessReports(object sender, EventArgs e) => ToggleAsync(IllnessReportsContent);

    /// <summary>
    /// A function that toggles the population geolocation data content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnPopulationGeolocationData(object sender, EventArgs e) => ToggleAsync(PopulationGeolocationDataContent);
    
    /// <summary>
    /// A function that toggles the location content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnLocation(object sender, EventArgs e) => ToggleAsync(LocationContent);

    /// <summary>
    /// A function that toggles the device information content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnDeviceInformation(object sender, EventArgs e) => ToggleAsync(DeviceInformationContent);

    /// <summary>
    /// A function that toggles the feedback content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnFeedback(object sender, EventArgs e) => ToggleAsync(FeedbackContent);

    /// <summary>
    /// A function that toggles the third party services content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnThirdPartyServices(object sender, EventArgs e) => ToggleAsync(ThirdPartyServicesContent);

    /// <summary>
    /// A function that toggles the user controls preferences content panel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickedOnUserControlsPreferences(object sender, EventArgs e) => ToggleAsync(UserControlsPreferencesContent);
}