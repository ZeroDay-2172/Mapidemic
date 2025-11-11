//Author(s): Connor McGuire

using Microcharts;
using SkiaSharp;
using Microcharts.Maui;
using Syncfusion;
using Mapidemic.Models;
using Syncfusion.Maui.Toolkit.Charts;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Mapidemic;

public partial class GraphPage : ContentPage
{
    private string selectedIllness = "";
    private bool localTrends = false;
    private bool localityChosen = false;
    public ObservableCollection<Illness> IllnessCollection { get; set; } = new();

    public GraphPage()
    {
        InitializeComponent();
        illnessPicker.BindingContext = this;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyGraphTheme();
        await LoadIllnesses();

        // Reset chart & illness selection
        selectedIllness = "";
        column.ItemsSource = null;
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
    /// Handler for button that refreshes graph's data, only if illness is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void RefreshButtonClicked(Object sender, EventArgs args)
    {
        if (localityChosen)
        {
            if (selectedIllness.Length > 0)
            {
                // Get the chatModel with the data included
                ChartModel chartModel = await ChartModel.CreateAsync(selectedIllness, localTrends);

                if (chartModel.data.Count != 0)
                {
                    // Reverse data to show most recent data on the right
                    chartModel.data.Reverse();
                    // Update graph to show data
                    column.ItemsSource = chartModel.data;
                }
                else
                    await DisplayAlert("Notification", "No reports found in last 7 days", "OK!");

                // ApplyGraphTheme();
            }
        }
        else
        {
            await DisplayAlert("Alert!", "Please specify a locality", "OK");
        }

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
        }
        else
        {
            dataChart.XAxes[0].LabelStyle.TextColor = Colors.Black;
            dataChart.XAxes[0].Title.TextColor = Colors.Black;
            dataChart.YAxes[0].LabelStyle.TextColor = Colors.Black;
            dataChart.YAxes[0].Title.TextColor = Colors.Black;
        }
    }
}