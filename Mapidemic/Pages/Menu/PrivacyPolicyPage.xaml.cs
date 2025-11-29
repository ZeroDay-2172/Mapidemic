namespace Mapidemic.Pages.Menu;

public partial class PrivacyPolicyPage : ContentPage
{
    public PrivacyPolicyPage()
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
    private async void ClickedOnReason(object sender, EventArgs e) => ToggleAsync(ReasonContent);
}