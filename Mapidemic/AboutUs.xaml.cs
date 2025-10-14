namespace Mapidemic;

public partial class AboutUs : ContentPage
{
    public AboutUs()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Shows the information of the selected user
    /// </summary>
    private async void ShowInfo(object sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        if (btn.CommandParameter is not View target) return;

        AlexGriep.IsVisible = false;
        ArthurWaldman.IsVisible = false;
        ConnorMcGuire.IsVisible = false;
        NengYang.IsVisible = false;

        target.IsVisible = true;
    }
}