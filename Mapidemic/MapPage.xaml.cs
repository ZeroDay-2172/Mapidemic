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
            try
            {
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
            var location = await MauiProgram.businessLogic.GetPostalCodeCentroids(postalCode.Value);
            if (location != null)
            {
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
    /// Get the centroid location for a given ZIP code, using cache if available.
    /// If not in cache, fetch from database or geocode as a fallback.
    /// </summary>
    /// <param name="zip"></param>
    /// <returns>Accurate or Approximate location for the ZIP code, null if not found</returns>
    private async Task<Location?> GetCentroidForZip(int zip)
    {
        if (_zipCenterCache.TryGetValue(zip, out var cachedLocation)) // Step 1: Check cache first
        {
            return cachedLocation;
        }

        var centroid = await MauiProgram.businessLogic.GetPostalCodeCentroids(zip); // Step 2: Fetch from database
        if (centroid != null)
        {
            var location = new Location(centroid.Latitude, centroid.Longitude);
            _zipCenterCache[zip] = location;
            return location;
        }

        try // Step 3: Fallback to geocoding service
        {
            string postalCode = zip.ToString("D5");
            var locations = await Geocoding.Default.GetLocationsAsync(postalCode);
            var location = locations?.FirstOrDefault();
            if (location != null)
            {
                var loc = new Location(location.Latitude, location.Longitude);
                _zipCenterCache[zip] = loc;
                return loc;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error geocoding postal code {zip}: {ex.Message}");
        }

        System.Diagnostics.Debug.WriteLine($"Warning: No centroid found for postal code {zip}."); // Step 4: Total failure
        return null;
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
                return; // No data to render! TODO: Negate and swap this logic e.g. if counts exist, render them
            }

            var grouped = counts
                .GroupBy(r => r.PostalCode)
                .Select(g => new { Zip = g.Key, Count = g.Sum(x => x.TotalCount) }) // Groups by postal code and sums the counts
                .ToList();

            foreach (var item in grouped)
            {

                var center = await GetCentroidForZip(item.Zip); // Get the centroid for the ZIP code
                if (center == null)
                {
                    continue; // Skip if no centroid found
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
            return (Colors.Orange, baseRadius); // more
        }
        return (Colors.Red, baseRadius); // severe
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