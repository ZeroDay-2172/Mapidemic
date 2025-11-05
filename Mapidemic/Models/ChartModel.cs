using System.Collections.ObjectModel;
using Mapidemic.Models;
using Microcharts;

namespace Mapidemic.Models;

public class ChartModel
{

    public List<ChartData> data { get; set; }

    const int numDays = 7;

    /// <summary>
    /// Private constructor for ChartModel, does nothing
    /// </summary>
    /// <param name="selectedIllness">Illness to display data on</param>
    /// <param name="localTrends">local data if true, otherwise national</param>
    private ChartModel()
    {
        // Instantiate data
        data = new List<ChartData>();
    }

    public static async Task<ChartModel> CreateAsync(string selectedIllness, bool localTrends)
    {
        var model = new ChartModel();
        await model.getData(selectedIllness, localTrends);
        return model;
    }
    
    /// <summary>
    /// Gets the data for the past numDays days, including the current
    /// </summary>
    /// <param name="selectedIllness"></param>
    /// <param name="localTrends"></param>
    public async Task getData(string selectedIllness, bool localTrends)
    {
        int[] results = new int[numDays];
        results = await getNumberOfReports(selectedIllness, DateTimeOffset.UtcNow, localTrends);

        // Add the data for previous numDays days including the current, to data
        for (int i = 0; i < numDays; i++)
        {
            data.Add(new ChartData(results[i], DateTimeOffset.UtcNow.AddDays(-i)));
        }
    }

    /// <summary>
    /// Gets number of cases for past numDays days, including current day
    /// </summary>
    /// <param name="selectedIllness">Illness to get data for</param>
    /// <param name="date">Current UTC Date</param>
    /// <param name="localTrends">Should trends be local?</param>
    /// <returns></returns>
    public async Task<int[]> getNumberOfReports(string selectedIllness, DateTimeOffset date, bool localTrends)
    {
        int[] result = new int[numDays];
        for (int i = 0; i < result.Length; i++) // Starting at current day, in descending order. i.e. Today(i), yesterday(i+1), day before yesterday(i+2)
        {
            try // attempting to get the number of reports from the database
            {
                result[i] = await MauiProgram.businessLogic.getNumberOfReports(selectedIllness, DateTimeOffset.UtcNow.AddDays(-i), localTrends);
            }
            catch (Exception) // putting zero for a given day if the database cannot be reached
            {
                result[i] = 0;
            }
        }
        return result;
    }
}