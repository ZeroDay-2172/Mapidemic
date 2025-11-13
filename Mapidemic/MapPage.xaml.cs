using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using CommunityToolkit.Mvvm.Messaging;
namespace Mapidemic;

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

    void OnLegendToggleClicked(object sender, EventArgs e)
    {
        LegendFrame.IsVisible = !LegendFrame.IsVisible; // Toggles the visibility of the legend frame
    }

    /// <summary>
    /// Center the map on the user's postal code.
    /// </summary>
    async Task CenterOnUserPostalCode()
    {
        var settings = MauiProgram.businessLogic.ReadSettings();
        int? postalCode = settings.PostalCode;
        try
        {
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
        catch (Exception error) // catching error if database could not be reached to get the list of centroids
        {
            await DisplayAlert("Network Error", $"{error.Message}", "OK");
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
        try // attempting to read in the postal code centroids
        {
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
        }
        catch (Exception error) // catching error if the database could not be reached for postal code centroids
        {
            await DisplayAlert("Network Error", $"{error.Message}", "OK");
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
            foreach (var c in _heatmapCircles)
            {
                MapControl.MapElements.Remove(c);
            }
            _heatmapCircles.Clear();

            var counts = await MauiProgram.businessLogic.GetZipIllnessCounts(); // Gets all the zip illness counts from the database, might be poorly optimized
            if (counts == null || counts.Count == 0)
            {
                return; // No data to render
            }

            var grouped = counts
                .GroupBy(r => r.PostalCode)
                .Select(g => new { Zip = g.Key, Count = g.Sum(x => x.TotalCount) }) // Groups by postal code and sums the counts
                .ToList();

            foreach (var item in grouped)
            {

                var center = await GetCentroidForZip(item.Zip);
                if (center == null)
                {
                    continue; // Skip if no centroid found
                }

                var population = await GetPopulation(item.Zip);
                var style = GetStyleForCount(item.Count, population);
                await DrawHeatmapCircles(center, style.radiusMiles, style.color);

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to render heatmap: {ex.Message}", "OK"); // Display an alert if rendering fails
        }
    }

    /// <summary>
    /// Map a report count and population to a color and radius. Thresholds are easy to tweak.
    /// </summary>
    private (Color color, double radiusMiles) GetStyleForCount(int count, int? population)
    {
        const double baseRadius = 2.5; // miles

        if (population.HasValue && population.Value > 0)
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
    /// Get the population count for a given postal code, using cache if available.
    /// </summary>
    private async Task<int?> GetPopulation(int postalCode)
    {
        if (_populationCache.TryGetValue(postalCode, out var cachedPopulation))
        {
            return cachedPopulation;
        }
        try
        {
            var population = await MauiProgram.businessLogic.GetPopulationCount(postalCode);
            _populationCache[postalCode] = population;
            return population;
        }
        catch (Exception error) // If the database could not be reached
        {
            await DisplayAlert("Network Error", $"{error.Message}", "OK");
            return null;
        }
    }

    /// <summary>
    /// Determine the incidence rate threshold based on count and population.
    /// Thresholds: 0-5% = 1, 6-10% = 2, 11-15% = 3, 16-20% = 4, >20% = 5
    /// </summary>
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