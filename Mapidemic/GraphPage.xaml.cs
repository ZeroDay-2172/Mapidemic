//Author(s): Connor McGuire

using Microcharts;
using SkiaSharp;
using Microcharts.Maui;
using Syncfusion;
using Android.Media.Metrics;
using Mapidemic.Models;
using Syncfusion.Maui.Toolkit.Charts;
using Android.Views;
using System.Diagnostics;
using Android.Media;

namespace Mapidemic;

public partial class GraphPage : ContentPage
{

    private string selectedIllness = "";
    private bool localTrends = false;

    public GraphPage()
    {
        InitializeComponent();
    }

    public async void illnessChosen_handler(Object sender, EventArgs args)
    {
        Picker illnessPicker = (Picker)sender;

        // Based on choice, set selectedIllness;
        int index = illnessPicker.SelectedIndex;
        switch (index)
        {
            case 0:
                selectedIllness = "Bronchitis";
                break;
            case 1:
                selectedIllness = "Chickenpox";
                break;
            case 2:
                selectedIllness = "Common Cold";
                break;
            case 3:
                selectedIllness = "COVID-19";
                break;
            case 4:
                selectedIllness = "E. Coli";
                break;
            case 5:
                selectedIllness = "Influenza";
                break;
            case 6:
                selectedIllness = "Listeria";
                break;
            case 7:
                selectedIllness = "Measles";
                break;
            case 8:
                selectedIllness = "Pneumonia";
                break;
            case 9:
                selectedIllness = "Salmonella";
                break;
            case 10:
                selectedIllness = "Strep Throat";
                break;
            case 11:
                selectedIllness = "Whooping Cough";
                break;
            default:
                selectedIllness = "";
                break;
        }
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