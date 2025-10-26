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
    private ObservableCollection<Illness> illnessCollection;

    public GraphPage()
    {
        InitializeComponent();

        SetIllnessesList();
    }

    /// <summary>
    /// Sets the illnesses in the picker
    /// </summary>
    public async void SetIllnessesList()
    {
        illnessCollection = new ObservableCollection<Illness>(await MauiProgram.businessLogic.GetIllnessList());
        illnessPicker.ItemsSource = illnessCollection;
    }

    /// <summary>
    /// Event handler for when an illness is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void illnessChosen_handler(Object sender, EventArgs args)
    {
        Picker illnessPicker = (Picker)sender;

        // Based on choice, set selectedIllness;
        int index = illnessPicker.SelectedIndex;
        selectedIllness = illnessCollection[index].Name!;
    }

    /// <summary>
    /// Event handler that determines if user wants data only for their zipcode,
    /// or if they want all national data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void localitySwitch_toggled(Object sender, EventArgs args)
    {
        if (localTrends == true)
            localTrends = false;
        else
            localTrends = true;
    }

    /// <summary>
    /// Handler for button that refreshes graph's data, only if illness is selected.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public async void refreshButtonClicked(Object sender, EventArgs args)
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
}