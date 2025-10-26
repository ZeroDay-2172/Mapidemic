namespace Mapidemic.Models;
using System.Collections.ObjectModel;
using Supabase.Postgrest;

public class Database
{
    private const string supabaseUrl = "https://aeqrpazberlimssdzviz.supabase.co";
    private const string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFlcXJwYXpiZXJsaW1zc2R6dml6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTk0NTE2NTQsImV4cCI6MjA3NTAyNzY1NH0.wRZ11nD7S9x-VAQo6KLewuRJpQvg0iFepFZ8dM9oCGM";
    public Supabase.Client supabaseClient;

    /// <summary>
    /// The default constructor of a Database
    /// </summary>
    public Database()
    {
        supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
        supabaseClient.InitializeAsync();
    }

    /// <summary>
    /// A function that accepts a postal code and
    /// queries the external database to validate its existence
    /// </summary>
    /// <param name="postalCode"></param>
    /// <returns>true if valid postal code, false if not</returns>
    public async Task<bool> ValidatePostalCode(int postalCode)
    {
        /// the list returned will contain the corresponding postal code, or nothing
        var response = await supabaseClient.From<PostalCode>().Where(x => x.Code == postalCode).Get();
        return response?.Models?.Count == 1;
    }

    /// <summary>
    /// A function that returns all symptoms that
    /// appear in the database
    /// </summary>
    /// <returns>An observable collection of all symptoms</returns>
    public async Task<List<Illness>> GetSymptomsList()
    {
        return (await supabaseClient.From<Illness>().Select("symptoms").Get()).Models;
    }

    /// <summary>
    /// A function that return all the illnesses
    /// that appear in the database
    /// </summary>
    /// <returns>A list of all illnesses</returns>
    public async Task<List<Illness>> GetIllnessList()
    {
        return (await supabaseClient.From<Illness>().Get()).Models;
    }

    /// <summary>
    /// A function that gets a list of illnesses from the database
    /// </summary>
    /// <returns></returns>
    public async Task<List<Illnesses>> GetIllnessesList()
    {
        var response = await supabaseClient.From<Illnesses>().Where(x => x.Illness != null).Get();
        return response.Models;
    }

    /// <summary>
    /// Returns the count of rows (reports) of an illness in the provided zip code, on the specific day.
    /// </summary>
    /// <param name="illnessName">Name of illness to find data on</param>
    /// <param name="date">Date of the report(s) to consider</param>
    /// <param name="postalCode">Specifies what area the report took place, -1 if national reports</param>
    /// <returns></returns>
    public async Task<int> getNumberOfReports(string illnessName, DateTimeOffset date, int postalCode)
    {
        var startOfDay = date.UtcDateTime.Date;
        var endOfDay = startOfDay.AddDays(1);

        // Show local reports
        if (postalCode != -1)
        {
            return await supabaseClient
                         .From<IllnessReport>()
                         .Where(x => x.PostalCode == postalCode)
                         .Where(x => x.IllnessType == illnessName)
                         .Where(x => x.ReportDate >= startOfDay && x.ReportDate < endOfDay)
                         .Count(Supabase.Postgrest.Constants.CountType.Exact);
        }
        // Show national reports
        else
        {
            return await supabaseClient
                         .From<IllnessReport>()
                         .Where(x => x.IllnessType == illnessName)
                         .Where(x => x.ReportDate >= startOfDay && x.ReportDate < endOfDay)
                         .Count(Supabase.Postgrest.Constants.CountType.Exact);
        }
    }
}