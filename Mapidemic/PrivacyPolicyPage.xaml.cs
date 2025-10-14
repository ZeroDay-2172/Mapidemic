namespace Mapidemic;

public partial class PrivacyPolicyPage : ContentPage
{
    public PrivacyPolicyPage()
    {
        InitializeComponent();
    }

    private async Task ToggleAsync(View panel)
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

    private async void ClickedOnIntro(object sender, EventArgs e) => await ToggleAsync(IntroContent);
    private async void ClickedOnReason(object sender, EventArgs e) => await ToggleAsync(ReasonContent);
}