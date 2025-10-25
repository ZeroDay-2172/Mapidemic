using Microsoft.Maui.Controls;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading;
using Mapidemic.Models;
namespace Mapidemic;

public partial class MapPage : ContentPage
{
    private readonly List<Circle> _heatmapCircles = new(); // Keep track of rendered heatmap circles so we can clear/update them
    private static readonly Dictionary<int, Location> _zipCenterCache = new(); // Cache ZIP -> geocoded center to reduce geocoding calls and rate limits
    private readonly SemaphoreSlim _renderLock = new(1, 1); // Lock for synchronizing map rendering
    private bool _refreshScheduled; // Flag to indicate if a refresh is already scheduled

    public MapPage() => InitializeComponent();

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EnsureLocationPermissionAsync(); // When the page appears, ensure permissions and center map
        await CenterOnUserLocationAsync();
        await RenderReportHeatmapAsync(); // Render the heatmap when the page appears

        WeakReferenceMessenger.Default.Register<object, string>(this, "IllnessReportedZip", async (sender, zip) =>
        {
            if (_refreshScheduled) // If a refresh is already scheduled, ignore new reports
                return;

            _refreshScheduled = true;
            await Task.Delay(2000); // Wait 2 seconds before refreshing
            await _renderLock.WaitAsync(); // Acquire the lock to ensure only one render at a time
            try
            {
                await RenderReportHeatmapAsync(); // Re-render the heatmap
            }
            finally
            {
                _refreshScheduled = false; // Reset the flag
                _renderLock.Release(); // Release the lock
            }
        }); // The code above took me forever to get right and my only hope is that it's so bad, I never have to handle threading again
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.UnregisterAll(this); // Unregister messenger when page disappears
    }


    /// <summary>
    /// Handle the Report Illness button click to navigate to the report page.
    /// </summary>
    void OnReportIllnessClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ReportIllnessPage()); // Navigate to the report illness page
    }

    /// <summary>
    /// Ensure the app has location permissions; if not, request them.
    /// </summary>
    async Task EnsureLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>(); // A simple check for iOS is needed for it to work
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
    /// Center the map on the user's location.
    /// </summary>
    async Task CenterOnUserLocationAsync() // This needs to be changed but for some reason, I can't get the zip from the settings, so I'm centering on location instead
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                var userPosition = new Location(location.Latitude, location.Longitude);
                MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(userPosition, Distance.FromMiles(1)));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Render color-coded circles by ZIP code, based on count of illness reports.
    /// Yellow = minimal, Red = more, Black = severe. Fixed radius to preserve privacy.
    /// </summary>
    private async Task RenderReportHeatmapAsync()
    {
        try
        {
            foreach (var c in _heatmapCircles)
            {
                MapControl.MapElements.Remove(c); // Clear existing circles
            }
            _heatmapCircles.Clear();

            var counts = await MauiProgram.businessLogic.GetZipIllnessCountsAsync(); // Fetch illness report counts by ZIP code
            if (counts == null || counts.Count == 0)
            {
                return;
            }

            var grouped = counts // Group by ZIP code and sum total counts across illness types
                .GroupBy(r => r.PostalCode)
                .Select(g => new { Zip = g.Key, Count = g.Sum(x => x.TotalCount) }) // I spent an hour here, wondering why my circles weren't updating
                .ToList();

            foreach (var item in grouped)
            {

                if (!_zipCenterCache.TryGetValue(item.Zip, out var center)) // Check cache first
                {
                    var zipText = item.Zip.ToString("D5"); // Ensure ZIP code is 5 digits
                    var loc = (await Geocoding.Default.GetLocationsAsync(zipText))?.FirstOrDefault(); // Geocode ZIP code
                    if (loc == null) // If geocoding fails, skip this ZIP
                        continue;
                    center = new Location(loc.Latitude, loc.Longitude); // Create location from geocoded result
                    _zipCenterCache[item.Zip] = center;
                }

                var style = GetStyleForCount(item.Count); // Determine color and radius based on count
                await DrawHeatmapCircles(center, style.radiusMiles, style.color); // Draw the circle on the map

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to render heatmap: {ex.Message}", "OK"); // If for any reason rendering fails, show an alert
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
}