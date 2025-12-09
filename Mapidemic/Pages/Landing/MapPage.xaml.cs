using System.Diagnostics;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Mapidemic.Pages.ReportIllness;
using CommunityToolkit.Mvvm.Messaging;
using Mapidemic.Pages.ReportIllness;
using Mapidemic.Models;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Mapidemic.Pages.Landing;

/// <summary>
/// A class that provides a user interface for tracking illness on a map
/// </summary>
public partial class MapPage : ContentPage
{
    private readonly List<Circle> _heatmapCircles = new();
    private readonly Dictionary<int, Location> _zipCenterCache = new();
    private readonly Dictionary<int, int?> _populationCache = new();
    private readonly SemaphoreSlim _renderLock = new(1, 1); // Semaphore is here to save the day, preventing concurrent renders
    private bool _refreshScheduled;
    private const int percentageMultiplier = 100;
    private const int minPercentageThreshold = 5;
    private const int moderatePercentageThreshold = 10;
    private const int morePercentageThreshold = 15;
    private const int maxPercentageThreshold = 20;
    private const int returnMinimalThreshold = 1;
    private const int returnModerateThreshold = 2;
    private const int returnMoreThreshold = 3;
    private const int returnSevereThreshold = 4;
    private const int returnCriticalThreshold = 5;
    private const int defaultMinimumThreshold = 2;
    private const int defaultModerateThreshold = 5;
    private const int defaultMoreThreshold = 10;
    private const int defaultSevereThreshold = 15;
    private const double viewportPaddingFactor = 1.2; // Factor to slightly expand the viewport for better visibility
    private const double maxZoomOutMiles = 50.0; // Maximum zoom out distance in miles
    private const double zoomNudgeFactor = .5; // Nudge factor to slightly zoom in when exceeding max zoom out
    private bool _isAdjustingZoom;
    private readonly Dictionary<Circle, (int Count, int? Population)> _circleMeta = new();

    /// <summary>
    /// The designated constructor for a MapPage
    /// </summary>
    public MapPage()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<object, string>(this, "IllnessReportedZip", async (sender, zip) =>
        {
            try // attempting to throttle the refresh rate of the heat map
            {
                await ThrottleRefreshRate(); // Throttle the refresh rate to avoid excessive updates
            }
            catch (Exception ex) // error if the throttle does not work
            {
                await HomePage.ShowPopup("Unable to refresh heatmap"); // Display an alert if refreshing fails
                Debug.WriteLine(ex.Message);
            }
        });
    }

    /// <summary>
    /// A function that centers the map and loads heat circles
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MapControl.PropertyChanged += OnMapPropertyChanged; // refresh when the user interacts with the map
        await CenterOnUserPostalCode();
        await RenderReportHeatmap();
        await RenderVisibleCircles();
    }

    /// <summary>
    /// Clean up messenger registrations when the page disappears.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MapControl.PropertyChanged -= OnMapPropertyChanged;
        WeakReferenceMessenger.Default.UnregisterAll(this); // If this isn't here, the app will crash because we can't re-register messengers
    }

    /// <summary>
    /// Handle the Report Illness button click to navigate to the report page.
    /// </summary>
    async void OnReportIllnessClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new ReportIllnessPage()); // Navigates to the report illness page
        }
        catch (Exception ex)
        {
            await DisplayAlert("Navigation Error", $"Unable to open report page: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// A function that shows the map legend
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnLegendToggleClicked(object sender, EventArgs e)
    {
        LegendFrame.IsVisible = !LegendFrame.IsVisible; // Toggles the visibility of the legend frame
    }

    /// <summary>
    /// A function that closes the map legend
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnLegendBackClicked(object sender, EventArgs e)
    {
        LegendFrame.IsVisible = false; // Hides the legend frame when the back button is clicked
    }

    /// <summary>
    /// Handle map property changes to enforce maximum zoom level and refresh visible circles.
    /// </summary>
    private async void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapControl.VisibleRegion))
        {
            var visibleRegion = MapControl.VisibleRegion;
            if (visibleRegion?.Radius != null && !_isAdjustingZoom && visibleRegion.Radius.Miles > maxZoomOutMiles)
            {
                try
                {
                    _isAdjustingZoom = true;
                    var center = visibleRegion.Center;
                    var targetMiles = Math.Max(1.0, maxZoomOutMiles * zoomNudgeFactor); // small inward nudge
                    MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(targetMiles)));
                }
                finally
                {
                    _isAdjustingZoom = false;
                }
                return;
            }
        }
        await RenderVisibleCircles();
    }

    /// <summary>
    /// Center the map on the user's postal code.
    /// </summary>
    async Task CenterOnUserPostalCode()
    {
        var settings = MauiProgram.businessLogic.ReadSettings();
        int? postalCode = settings.PostalCode;
        try // attempting to get the centroid for the settings postal code
        {
            if (postalCode != null) // getting the centroid if settings contains a postal code
            {
                var location = await MauiProgram.businessLogic.GetPostalCodeCentroids(postalCode.Value);
                if (location != null)
                {
                    var center = new Location(location.Latitude, location.Longitude); // Gathers the map on internal centroids given by Census.gov, wacky postal codes have odd centroids that will never be 100% accurate. Downside of user privacy.
                    MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(10)));
                }
            }
            else // if no postal code, centering on the last known location
            {
                var location = await Geolocation.Default.GetLastKnownLocationAsync(); // If for some reason postal code isn't set, fall back to device location. Look into adding a default location for the most private of users.
                if (location != null)
                {
                    var center = new Location(location.Latitude, location.Longitude);
                    MapControl.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(10)));
                }
            }
        }
        catch (Exception error) // catching error if database could not be reached to get the list of centroids
        {
            await HomePage.ShowPopup("Unable to center map");
            Debug.WriteLine(error.Message);
        }
    }

    /// <summary>
    /// Render color-coded circles by ZIP code, based on count of illness reports.
    /// Color scheme: Yellow → Gold → Orange → OrangeRed → Red, with Red indicating the highest severity. Fixed radius to preserve privacy.
    /// </summary>
    private async Task RenderReportHeatmap()
    {
        try // attempting to render the heat map circles
        {
            foreach (var c in _heatmapCircles)
            {
                MapControl.MapElements.Remove(c);
            }
            _heatmapCircles.Clear();

            // var counts = await MauiProgram.businessLogic.GetZipIllnessCounts(); // Gets all the zip illness counts from the database, might be poorly optimized
            var counts = await MauiProgram.businessLogic.GetActiveZipIllnessCounts(); // Filters to only active illness reports
            if (counts == null || counts.Count == 0)
            {
                return; // No data to render
            }

            var grouped = counts
                .GroupBy(r => r.PostalCode)
                .Select(g => new { Zip = g.Key, Count = g.Sum(x => x.TotalCount) }) // Groups by postal code and sums the counts
                .ToList();

            var zipCodes = grouped.Select(g => g.Zip).ToList();
            var centroidDict = await MauiProgram.businessLogic.GetPostalCodeCentroidsBulk(zipCodes); // Bulk fetch centroids for all relevant ZIP codes
            var populationDict = await MauiProgram.businessLogic.GetPopulationCountsBulk(zipCodes); // Bulk fetch populations for all relevant ZIP codes
            var visibleRegion = MapControl.VisibleRegion;

            foreach (var item in grouped)
            {
                if (!centroidDict.TryGetValue(item.Zip, out var centroid))
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: No centroid found for postal code {item.Zip}. Skipping.");
                    continue; // Skip if no centroid found
                }
                var center = new Location(centroid.Latitude, centroid.Longitude);
                _zipCenterCache[item.Zip] = center; // Cache the location
                populationDict.TryGetValue(item.Zip, out var population); // Try to get population, may be null
                _populationCache[item.Zip] = population?.PopulationCount; // Cache population count if available

                if (visibleRegion != null && !IsInVisibleRegion(center, visibleRegion)) // Only draw if in visible region
                {
                    _heatmapCircles.Add(new Circle // Still create the circle for caching, but don't add it to the map yet
                    {
                        Center = center,
                        Radius = Distance.FromMiles(GetStyleForCount(item.Count, population?.PopulationCount).radiusMiles),
                        StrokeWidth = 2,
                        StrokeColor = GetStyleForCount(item.Count, population?.PopulationCount).color.WithAlpha(0.9f),
                        FillColor = GetStyleForCount(item.Count, population?.PopulationCount).color.WithAlpha(0.35f),
                    });
                    _circleMeta[_heatmapCircles.Last()] = (item.Count, population?.PopulationCount);
                    continue; // Skip if not in visible region
                }

                var style = GetStyleForCount(item.Count, population?.PopulationCount);
                await DrawHeatmapCircles(center, style.radiusMiles, style.color); // Draw the circle on the map
            }

            await RenderVisibleCircles(); // Ensure only visible circles are rendered
        }
        catch (Exception ex) // unable to render the heatmap
        {
            await HomePage.ShowPopup("Unable to render heatmap");
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Render only the circles that are within the current visible region of the map.
    /// </summary>
    /// <returns></returns>
    private async Task RenderVisibleCircles()
    {
        try
        {
            var visibleRegion = MapControl.VisibleRegion;
            if (visibleRegion == null)
            {
                return;
            }

            foreach (var circle in _heatmapCircles)
            {
                bool inside = IsInVisibleRegion(circle.Center, visibleRegion); // Check if circle center is in visible region
                if (inside)
                {
                    if (!MapControl.MapElements.Contains(circle)) // Add circles that are inside the visible region
                    {
                        MapControl.MapElements.Add(circle);
                    }
                }
                else
                {
                    if (MapControl.MapElements.Contains(circle)) // Remove circles that are outside the visible region
                    {
                        MapControl.MapElements.Remove(circle);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to render visible circles: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Check if a location is within the visible region of the map.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="visibleRegion"></param>
    /// <returns></returns>
    private static bool IsInVisibleRegion(Location location, MapSpan visibleRegion)
    {
        double halfLat = visibleRegion.LatitudeDegrees / 2 * viewportPaddingFactor;
        double halfLon = visibleRegion.LongitudeDegrees / 2 * viewportPaddingFactor;
        double north = visibleRegion.Center.Latitude + halfLat;
        double south = visibleRegion.Center.Latitude - halfLat;
        double east = visibleRegion.Center.Longitude + halfLon;
        double west = visibleRegion.Center.Longitude - halfLon;
        return location.Latitude <= north && location.Latitude >= south &&
               location.Longitude <= east && location.Longitude >= west;
    }

    /// <summary>
    /// Map a report count and population to a color and radius. Thresholds are easy to tweak.
    /// </summary>
    private (Color color, double radiusMiles) GetStyleForCount(int count, int? population)
    {
        const double baseRadius = 2.5; // miles

        if (population.HasValue && population.Value > 0) // coloring circles based on population size
        {
            int incidenceRate = DetermineRateThreshold(count, population.Value);

            switch (incidenceRate)
            {
                case <= returnMinimalThreshold:
                    return (Colors.Yellow, baseRadius); // minimal
                case <= returnModerateThreshold:
                    return (Colors.Gold, baseRadius); // moderate
                case <= returnMoreThreshold:
                    return (Colors.Orange, baseRadius); // more
                case <= returnSevereThreshold:
                    return (Colors.OrangeRed, baseRadius); // severe
                case <= returnCriticalThreshold:
                    return (Colors.Red, baseRadius); // dangerous
                default:
                    break; // fallback to count-based styling
            }
        }
        switch (count) // This switch case will only execute if population data is unavailable
        {
            case <= defaultMinimumThreshold:
                return (Colors.Yellow, baseRadius);
            case <= defaultModerateThreshold:
                return (Colors.Gold, baseRadius);
            case <= defaultMoreThreshold:
                return (Colors.Orange, baseRadius);
            case <= defaultSevereThreshold:
                return (Colors.OrangeRed, baseRadius);
            default:
                return (Colors.Red, baseRadius);
        }
    }

    /// <summary>
    /// Determine the incidence rate threshold based on count and population.
    /// Thresholds: 0-5% = 1, 6-10% = 2, 11-15% = 3, 16-20% = 4, >20% = 5
    /// </summary>
    /// <param name="count"></param>
    /// <param name="population"></param>
    /// <returns></returns>
    private int DetermineRateThreshold(int count, int population)
    {
        double percent = (double)count / population * percentageMultiplier; // Since GetStyleForCount relies on the population being non-null, we don't need to check for null here.

        if (percent <= minPercentageThreshold)
            return returnMinimalThreshold;
        else if (percent <= moderatePercentageThreshold)
            return returnModerateThreshold;
        else if (percent <= morePercentageThreshold)
            return returnMoreThreshold;
        else if (percent <= maxPercentageThreshold)
            return returnSevereThreshold;
        else
            return returnCriticalThreshold;
    }

    /// <summary>
    /// Draw heatmap circles on the map based on illness report data.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private async Task DrawHeatmapCircles(Location center, double radius, Color color)
    {
        var circle = new Circle
        {
            Center = center,
            Radius = Distance.FromMiles(radius),
            StrokeWidth = 2,
            StrokeColor = color.WithAlpha(0.9f),
            FillColor = color.WithAlpha(0.35f),
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