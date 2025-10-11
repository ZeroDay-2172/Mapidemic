namespace Mapidemic.Models;
using System.Collections.ObjectModel;

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
}