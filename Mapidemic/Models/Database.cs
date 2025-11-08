using Xamarin.Google.ErrorProne.Annotations;

namespace Mapidemic.Models;

public class Database
{
    private const int timeoutDuration = 5;
    private const string networkError = "Network error! Please try again shortly.";
    private const string supabaseUrl = "https://aeqrpazberlimssdzviz.supabase.co";
    private const string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFlcXJwYXpiZXJsaW1zc2R6dml6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTk0NTE2NTQsImV4cCI6MjA3NTAyNzY1NH0.wRZ11nD7S9x-VAQo6KLewuRJpQvg0iFepFZ8dM9oCGM";
    public Supabase.Client supabaseClient;
    private Supabase.Postgrest.Constants.Ordering ascending = Supabase.Postgrest.Constants.Ordering.Ascending; //Add as second parameter in Order method of query to order ascending

    /// <summary>
    /// The default constructor of a Database
    /// </summary>
    public Database()
    {
        supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
        supabaseClient.InitializeAsync();
    }

    /// <summary>
    /// A generic function that accepts any kind of query for
    /// the database and returns the result within 5 seconds.
    /// This function prevents hanging connection time when
    /// the user is disconnected from the internet or gets
    /// blocked by the network they are on
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <returns>the result of the query</returns>
    /// <exception cref="Exception">database error</exception>
    private async Task<T> IssueQuery<T>(Task<T> query)
    {
        var completedQuery = await Task.WhenAny(query, Task.Delay(TimeSpan.FromSeconds(timeoutDuration))); // setting the connection time to 5 seconds
        if (completedQuery == query) // ensuring the query finishes in time
        {
            return await query;
        }
        else // error if the query did not finish in time
        {
            throw new Exception(networkError);
        }
    }

    /// <summary>
    /// A function that tests the database connection
    /// </summary>
    /// <returns>true is connection valid, false if not</returns>
    public async Task<bool> TestConnection()
    {
        try // attempting a query
        {
            return (await IssueQuery(supabaseClient.From<PostalCode>().Where(x => x.Code == 601).Get())).Model!.Code == 601;
        }
        catch (Exception) // returning false if query failed
        {
            return false;
        }
    }

    /// <summary>
    /// A function that accepts a postal code and
    /// queries the external database to validate its existence
    /// </summary>
    /// <param name="postalCode"></param>
    /// <returns>true if valid postal code, false if not</returns>
    public async Task<bool> ValidatePostalCode(int postalCode)
    {
        try // querying the database for the user's postal code
        {
            return (await IssueQuery(supabaseClient.From<PostalCode>().Where(x => x.Code == postalCode).Get())).Models.Count == 1;
        }
        catch(Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that returns all symptoms that
    /// appear in the database
    /// </summary>
    /// <returns>a list of all symptoms</returns>
    public async Task<List<Illness>> GetSymptomsList()
    {
        try // querying the database for the symptoms list
        {
            return (await IssueQuery(supabaseClient.From<Illness>().Select("symptoms").Get())).Models;
        }
        catch(Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that gets a list of illnesses from the database
    /// </summary>
    /// <returns>a list of all illnesses in the database</returns>
    public async Task<List<Illness>> GetIllnessesList()
    {
        try // querying the database for the illness list
        {
            return (await IssueQuery(supabaseClient.From<Illness>().Where(x => x.Name != null).Order("illness", ascending).Get())).Models;
        }
        catch (Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that gets a list containing each postal
    /// code that has one or more illness reports
    /// </summary>
    /// <returns>a list of postal codes and illness reports count values</returns>
    public async Task<List<ZipIllnessCounts>> GetZipIllnessCounts()
    {
        try // querying the database for the postal code and illness reports count list
        {
            return (await IssueQuery(supabaseClient.From<ZipIllnessCounts>().Select("*").Get())).Models;
        }
        catch (Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that gets the centroid values on the map
    /// for each postal code in the United States
    /// </summary>
    /// <param name="postalCode"></param>
    /// <returns>a list containing all the centroid values</returns>
    public async Task<List<PostalCodeCentroids>> GetPostalCodeCentroids(int postalCode)
    {
        try // querying the database for the list of postal code centroids
        {
            // var result = (await supabaseClient.From<PostalCodeCentroids>().Filter<List<int>>("postal_code", Supabase.Postgrest.Constants.Operator.In, [54901, 54902]).Get()).Models;
            // Console.WriteLine($"-------------------- {result[0].Code} : {result[0].Latitude} : {result[0].Longitude} ---------------------- {result[1].Code} : {result[1].Latitude} : {result[1].Longitude} ------------------------------------------------------------------------");
            return (await IssueQuery(supabaseClient.From<PostalCodeCentroids>().Where(x => x.Code == postalCode).Get())).Models;
        }
        catch (Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// A function that returns a list of illness reports
    /// based on the postal code
    /// </summary>
    /// <param name="postalCode"></param>
    /// <param name="daysPicked"></param>
    /// <returns>A list of illnesses for the postal code</returns>
    public async Task<List<IllnessReport>> GenerateReport(int postalCode, int daysPicked)
    {
        var days = DateTimeOffset.UtcNow.AddDays(-daysPicked); // calculating requested user illness window
        try // querying the database for reported illness for a specific postal code within the a specified time period
        {
            return (await IssueQuery(supabaseClient.From<IllnessReport>().Where(x => x.PostalCode == postalCode && x.ReportDate >= days).Get())).Models;
        }
        catch (Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }

    /// <summary>
    /// Returns the count of rows (reports) of an illness in the provided zip code, on the specific day.
    /// </summary>
    /// <param name="illnessName">Name of illness to find data on</param>
    /// <param name="date">Date of the report(s) to consider</param>
    /// <param name="postalCode">Specifies what area the report took place, -1 if national reports</param>
    /// <returns></returns>
    public async Task<int> GetNumberOfReports(string illnessName, DateTimeOffset date, int postalCode)
    {
        var startOfDay = date.UtcDateTime.Date;
        var endOfDay = startOfDay.AddDays(1);
        try // attempting to get total illness reports from the database
        {
            if (postalCode != -1) // show local reports
            {
                return await IssueQuery(supabaseClient
                                        .From<IllnessReport>()
                                        .Where(x => x.PostalCode == postalCode)
                                        .Where(x => x.IllnessType == illnessName)
                                        .Where(x => x.ReportDate >= startOfDay && x.ReportDate < endOfDay)
                                        .Count(Supabase.Postgrest.Constants.CountType.Exact));
            }
            else // show national reports
            {
                return await IssueQuery(supabaseClient
                                        .From<IllnessReport>()
                                        .Where(x => x.IllnessType == illnessName)
                                        .Where(x => x.ReportDate >= startOfDay && x.ReportDate < endOfDay)
                                        .Count(Supabase.Postgrest.Constants.CountType.Exact));
            }
        }
        catch(Exception error) // exception if the database cannot be reached
        {
            throw new Exception(error.Message);
        }
    }
}