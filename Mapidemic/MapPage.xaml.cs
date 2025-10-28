using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using CommunityToolkit.Mvvm.Messaging;
namespace Mapidemic;

public partial class MapPage : ContentPage
{
    private readonly List<Circle> _heatmapCircles = new();
    private static readonly Dictionary<int, Location> _zipCenterCache = new();
    private readonly SemaphoreSlim _renderLock = new(1, 1); // Semaphore is here to save the day, preventing concurrent renders
    private bool _refreshScheduled;

    public MapPage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<object, string>(this, "IllnessReportedZip", async (sender, zip) =>
        {
            try {
                await ThrottleRefreshRate(); // Throttle the refresh rate to avoid excessive updates
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to refresh heatmap: {ex.Message}", "OK"); // Display an alert if refreshing fails
            }
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EnsureLocationPermission();
        await CenterOnUserPostalCode();
        await RenderReportHeatmap();
    }

    /// <summary>
    /// Clean up messenger registrations when the page disappears.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.UnregisterAll(this); // If this isn't here, the app will crash because we can't re-register messengers
    }


    /// <summary>
    /// Handle the Report Illness button click to navigate to the report page.
    /// </summary>
    void OnReportIllnessClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ReportIllnessPage()); // Navigates to the report illness page
    }

    /// <summary>
    /// Ensure the app has location permissions; if not, request them.
    /// </summary>
    async Task EnsureLocationPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>(); // Might move this down to CenterOnUserPostalCode
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
    /// Center the map on the user's postal code.
    /// </summary>
    async Task CenterOnUserPostalCode()
    {
        var settings = MauiProgram.businessLogic.ReadSettings();
        int? postalCode = settings.PostalCode;
        if (postalCode != null)
        {
            var locations = await MauiProgram.businessLogic.GetPostalCodeCentroids(postalCode.Value);
            if (locations != null && locations.Count > 0)
            {
                var location = locations[0];
                var center = new Location(location.Latitude, location.Longitude); // Gathers the map on internal centroids given by Census.gov, wacky postal codes have odd centroids that will never be 100% accurate. Downside of user privacy.
                MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(10)));
            }
        }
        else
        {
            var location = await Geolocation.Default.GetLastKnownLocationAsync(); // If for some reason postal code isn't set, fall back to device location. Look into adding a default location for the most private of users.
            if (location != null)
            {
                var center = new Location(location.Latitude, location.Longitude);
                MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(10)));
            }
        }

    }

    /// <summary>
    /// Render color-coded circles by ZIP code, based on count of illness reports.
    /// Yellow = minimal, Red = more, Black = severe. Fixed radius to preserve privacy.
    /// </summary>
    private async Task RenderReportHeatmap()
    {
        try
        {
            foreach (var c in _heatmapCircles) // Looks into it's local cache 
            {
                MapControl.MapElements.Remove(c); // Removes all existing circles, to prepare for re-rendering
            }
            _heatmapCircles.Clear(); // Clears the local cache of heatmap circles

            var counts = await MauiProgram.businessLogic.GetZipIllnessCounts(); // Gets all the zip illness counts from the database, might be poorly optimized
            if (counts == null || counts.Count == 0)
            {
                return; // No data to render!
            }

            var grouped = counts
                .GroupBy(r => r.PostalCode)
                .Select(g => new { Zip = g.Key, Count = g.Sum(x => x.TotalCount) }) // Groups by postal code and sums the counts
                .ToList();

            foreach (var item in grouped)
            {

                if (!_zipCenterCache.TryGetValue(item.Zip, out var center)) // Check cache first
                {
                    var location = (await MauiProgram.businessLogic.GetPostalCodeCentroids(item.Zip)).FirstOrDefault(); // Fetch centroid from database if not in cache
                    if (location == null)
                        continue;
                    center = new Location(location.Latitude, location.Longitude);
                    _zipCenterCache[item.Zip] = center;
                }

                var style = GetStyleForCount(item.Count);
                await DrawHeatmapCircles(center, style.radiusMiles, style.color); // Draw the circle on the map

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to render heatmap: {ex.Message}", "OK"); // Display an alert if rendering fails
        }
    }

    /// <summary>
    /// Map a report count to a color and radius. Thresholds are easy to tweak.
    /// </summary>
    private (Color color, double radiusMiles) GetStyleForCount(int count)
    {
        const double baseRadius = 2.5; // miles

        if (count <= 2)
        {
            return (Colors.Yellow, baseRadius); // minimal
        }

        if (count <= 7)
        {
            return (Colors.Red, baseRadius); // more
        }
        return (Colors.Black, baseRadius); // severe
    }

    /// <summary>
    /// Draw heatmap circles on the map based on illness report data.
    /// </summary>
    private async Task DrawHeatmapCircles(Location Center, double Radius, Color Color)
    {
        var circle = new Circle
        {
            Center = Center,
            Radius = Distance.FromMiles(Radius),
            StrokeWidth = 2,
            StrokeColor = Color.WithAlpha(0.9f),
            FillColor = Color.WithAlpha(0.35f)
        };

        MapControl.MapElements.Add(circle);
        _heatmapCircles.Add(circle);
    }

    /// <summary>
    /// Throttle the refresh rate of the heatmap rendering to avoid excessive updates.
    /// SemaphoreSlim is used to prevent concurrent renders from happening over and over.
    /// What this does is ensure that if multiple requests to refresh the heatmap come in rapid succession,
    /// only one render operation is performed after a short delay, reducing unnecessary processing and improving performance.
    /// This method will only run after a report has been received.
    /// </summary>
    private async Task ThrottleRefreshRate()
    {
        if (_refreshScheduled)
            return;
        _refreshScheduled = true;
        await Task.Delay(2000);
        await _renderLock.WaitAsync();
        try
        {
            await RenderReportHeatmap();
        }
        finally
        {
            _refreshScheduled = false;
            _renderLock.Release();
        }
    }
}