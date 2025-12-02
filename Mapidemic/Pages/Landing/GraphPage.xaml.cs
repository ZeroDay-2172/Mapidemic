using Mapidemic.Models;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
namespace Mapidemic.Pages.Landing;

public partial class GraphPage : ContentPage
{
    private string selectedIllness = "";
    private bool localTrends = false;
    private bool localityChosen = false;

    // Note that 0 indicates no selection
    private int numDays = 0;
    public ObservableCollection<Illness> IllnessCollection { get; set; } = new();

    public ObservableCollection<string> LocalityCollection { get; set; } = new();

    public GraphPage()
    {
        InitializeComponent();
        illnessPicker.BindingContext = this;
        localityPicker.BindingContext = this;
    }

    /// <summary>
    /// Sets behavior of GraphPage upon appearing to the user.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyGraphTheme();
        await LoadIllnesses();

        // Reset chart & illness selection
        selectedIllness = "";
        chartTitle.Text = "No Data Loaded";
        column.ItemsSource = null;

        // Get list of localities -- Can be modified later
        string postalCode = MauiProgram.businessLogic.ReadSettings().PostalCode.ToString();
        LocalityCollection.Add("National");

        // Handle postalCodes with leading zeros
        switch (postalCode.Length)
        {
            case 3:
            LocalityCollection.Add("00" + postalCode);
            break;

            case 4:
            LocalityCollection.Add("0" + postalCode);
            break;

            default:
            LocalityCollection.Add(postalCode);
            break;
        }

        // Set default to zip code, and 7 days
        localTrends = true;
        localityChosen = true;
        localityPicker.SelectedIndex = 1;
        numDays = 7;
        timeRangePicker.SelectedIndex = 0;
    }

    /// <summary>
    /// Sets the illnesses in the picker
    /// </summary>
    public async Task LoadIllnesses()
    {
        try // attempting to read the illness list from the database
        {
            var illnesses = await MauiProgram.businessLogic.GetIllnessesList();
            IllnessCollection.Clear();
            foreach (var illness in illnesses)
            {
                IllnessCollection.Add(illness);
            }
        }
        catch (Exception error) // catching error if the database could not be reached
        {
            await DisplayAlert("Network Error", $"{error.Message}", "OK");
        }
    }

    /// <summary>
    /// Event handler for when an illness is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void IllnessChosen_handler(Object sender, EventArgs args)
    {
        Picker illnessPicker = (Picker)sender;

        // Based on choice, set selectedIllness;
        int index = illnessPicker.SelectedIndex;
        if (index >= 0 && IllnessCollection != null && index < IllnessCollection.Count)
        {
            selectedIllness = IllnessCollection[index].Name ?? string.Empty;
        }
    }

    /// <summary>
    /// Event handler that determines if user wants data only for their zipcode,
    /// or if they want all national data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void LocalityPicked_handler(Object sender, EventArgs args)
    {
        if (localityPicker.SelectedIndex == 0)
        {
            localTrends = false;
            localityChosen = true;
        }
        else if (localityPicker.SelectedIndex == 1)
        {
            localTrends = true;
            localityChosen = true;
        }
        else
            localityChosen = false;
    }

    /// <summary>
    /// EH for a time range being selected for the graph data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void TimeRangePicked_handler(Object sender, EventArgs args)
    {
        switch(timeRangePicker.SelectedIndex)
        {
            case 0:
                numDays = 7;
                break;
            case 1:
                numDays = 14;
                break;
            case 2:
                numDays = 30;
                break;
            default:
                numDays = 0;
                break;
        }
    }

    /// <summary>
    /// Handler for button that refreshes graph's data, only if illness is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void RefreshButtonClicked(Object sender, EventArgs args)
    {
        Popup.IsOpen = true;
        if (localityChosen) // Locality is chosen
        {
            if (numDays != 0) // Number of days is chosen
            {
                if (selectedIllness.Length > 0)
                {
                    // Get the chatModel with the data included
                    await Task.Yield();
                    ChartModel chartModel = await ChartModel.CreateAsync(selectedIllness, localTrends, numDays);

                    if (chartModel.data.Count != 0)
                    {
                        // Ensure there are not excessive horizontal lines on graph
                        SetGraphInterval(chartModel);
                        // Reverse data to show most recent data on the right
                        chartModel.data.Reverse();
                        // Update graph to show data
                        column.ItemsSource = chartModel.data;
                        // Set the title for the graph
                        SetGraphTitle();
                    }
                    else
                        await ShowPopup("No reports found in last 7 days");
                }
            }
        }
        Popup.IsOpen = false;
    }

    /// <summary>
    /// Method to apply current user theme to the graph.
    /// </summary>
    public async void ApplyGraphTheme()
    {
        if (Application.Current!.UserAppTheme == AppTheme.Dark)
        {
            dataChart.XAxes[0].LabelStyle.TextColor = Colors.White;
            dataChart.XAxes[0].Title.TextColor = Colors.White;
            dataChart.YAxes[0].LabelStyle.TextColor = Colors.White;
            dataChart.YAxes[0].Title.TextColor = Colors.White;
            chartTitle.TextColor = Colors.White;
        }
        else
        {
            dataChart.XAxes[0].LabelStyle.TextColor = Colors.Black;
            dataChart.XAxes[0].Title.TextColor = Colors.Black;
            dataChart.YAxes[0].LabelStyle.TextColor = Colors.Black;
            dataChart.YAxes[0].Title.TextColor = Colors.Black;
            chartTitle.TextColor = Colors.Black;
        }
    }

    /// <summary>
    /// Method to prevent excessive horizontal lines on graph.
    /// </summary>
    /// <param name="chartModel"></param>
    public void SetGraphInterval(ChartModel chartModel)
    {
        int maxIndex = -1;
        int max = -1;
        for (int i = 0; i < chartModel.data.Count; i++)
        {
            if (chartModel.data.ElementAt(i).value > max)
            {
                max = chartModel.data.ElementAt(i).value;
                maxIndex = i;
            }
        }

        // Ensure at most 10 lines, round to next larger int to prevent the cutoff of values
        if (chartModel.data.ElementAt(maxIndex).value > 10)
        {
            numericalAxis.Interval = chartModel.data.ElementAt(maxIndex).value / 10;
            numericalAxis.Interval = Math.Ceiling(numericalAxis.Interval);
        }
    }

    /// <summary>
    /// Method that sets the graph's title after the data has been fetched
    /// </summary>
    private async void SetGraphTitle()
    {
        String newTitle = selectedIllness + " reports for previous " + numDays + " days in ";

        bool isDarkMode = (Application.Current!.UserAppTheme == AppTheme.Dark);

        // Set the text for the title
        if (localTrends)
            newTitle = newTitle + "" + MauiProgram.businessLogic.ReadSettings().PostalCode;
        else
            newTitle = newTitle + "the US";

        // Update the actual title's text
        chartTitle.Text = newTitle;
    }

    private async Task ShowPopup(string message)
    {
        var popup = Toast.Make(
            message, 
            ToastDuration.Short,
            textSize: 18
        );
        await popup.Show();
    }
}