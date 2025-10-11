using Microsoft.Maui.Controls;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.ApplicationModel;
namespace Mapidemic;

public partial class MapPage : ContentPage
{
    public MapPage() => InitializeComponent();

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // When the page appears, ensure permissions and center map
        await EnsureLocationPermissionAsync();
        await CenterOnUserLocationAsync();
    }

    void OnReportIllnessClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ReportIllnessPage()); // Navigate to the report illness page
    }

    /// <summary>
    /// Ensure the app has location permissions; if not, request them.
    /// </summary>
    async Task EnsureLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Denied", "Location permission is required to show your current location on the map.", "OK");
        }
    }

    /// <summary>
    /// Center the map on the user's current location.
    /// </summary>
    async Task CenterOnUserLocationAsync()
    {
        try
        {
            var req = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var loc = await Geolocation.GetLocationAsync(req) ?? await Geolocation.GetLastKnownLocationAsync();
            if (loc == null)
            {
                await DisplayAlert("Location Error", "Unable to get your current location.", "OK");
                return;
            }
            var center = new Location(loc.Latitude, loc.Longitude);
            MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(0.3)));
            MapControl.IsShowingUser = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while retrieving location: {ex.Message}", "OK");
        }
    }

}