namespace Mapidemic.Pages.Menu;

public partial class InformationWeCollect : ContentPage
{
    public InformationWeCollect()
    {
        InitializeComponent();
    }

    private void ToggleAsync(View panel)
    {
        if (panel.IsVisible)
        {
            panel.IsVisible = false;
        }
        else
        {
            panel.IsVisible = true;
        }
    }

    private async void ClickedOnIntro(object sender, EventArgs e) => ToggleAsync(IntroContent);
    private async void ClickedOnIllnessReports(object sender, EventArgs e) => ToggleAsync(IllnessReportsContent);
    private async void ClickedOnPopulationGeolocationData(object sender, EventArgs e) => ToggleAsync(PopulationGeolocationDataContent);
    private async void ClickedOnLocation(object sender, EventArgs e) => ToggleAsync(LocationContent);
    private async void ClickedOnDeviceInformation(object sender, EventArgs e) => ToggleAsync(DeviceInformationContent);
    private async void ClickedOnFeedback(object sender, EventArgs e) => ToggleAsync(FeedbackContent);
    private async void ClickedOnThirdPartyServices(object sender, EventArgs e) => ToggleAsync(ThirdPartyServicesContent);
    private async void ClickedOnUserControlsPreferences(object sender, EventArgs e) => ToggleAsync(UserControlsPreferencesContent);

}