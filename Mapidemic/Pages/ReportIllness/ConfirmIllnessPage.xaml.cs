using Microsoft.Maui.Maps;

namespace Mapidemic.Pages.ReportIllness;

public partial class ConfirmIllnessPage : ContentPage
{
    private readonly string _illness;
    private readonly string _zip;

    public ConfirmIllnessPage(string illness, string zip) // Constructor to initialize with illness and ZIP code
    {
        InitializeComponent();
        _illness = illness;
        _zip = zip; // Takes the previously given variables and stores them

        SummaryLabel.Text = $"You are reporting {_illness} in the area with ZIP code {_zip}."; // Update the summary label with the provided information
    }

    /// <summary>
    /// Load and display the map centered on the provided ZIP code when the page appears
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            var loc = (await Geocoding.GetLocationsAsync(_zip))?.FirstOrDefault(); // Get the geographic location for the provided ZIP code
            if (loc != null)
            {
                var center = new Location(loc.Latitude, loc.Longitude);
                MapPreview.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMiles(1))); // Center the map on the location with a 1-mile radius
            }
            else
            {
                await DisplayAlert("Error", "Unable to find location for the provided ZIP code.", "OK"); // Show an error if the location cannot be found
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while retrieving location: {ex.Message}", "OK"); // Show an error if there is an exception
        }
        finally
        {
            BusyIndicator.IsRunning = BusyIndicator.IsVisible = false; // Hide the busy indicator
        }
    }

    /// <summary>
    /// Handle the edit button click event to go back and modify the report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnEditClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Go back to the previous page to edit
    }

    /// <summary>
    /// Handle the confirm button click event to submit the report
    /// </summary>
    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(_zip, out var zipInt))
        {
            await DisplayAlert("Error", "Invalid ZIP code format.", "OK"); // Basic check for ZIP code validity
            return;
        }

        var success = await MauiProgram.businessLogic.ReportIllness(Guid.NewGuid(), zipInt, _illness, DateTimeOffset.UtcNow); // Submit the report using the business logic layer
        if (!success)
        {
            await DisplayAlert("Error", "Submit failed. Please try again later.", "OK"); // Show an error if submission fails
            return;
        }
        await DisplayAlert("Success", "Your illness report has been submitted successfully.", "OK"); // Show a success message
        await Navigation.PopToRootAsync(); // Go back to the main map page
    }
}
