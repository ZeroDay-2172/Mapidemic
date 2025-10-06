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
        // burning the first query for speed up
        supabaseClient.From<PostalCode>().Where(x => x.Code == 00601).Get();
    }

    /// <summary>
    /// A function that accepts a postal code and
    /// queries the external database to validate its existence
    /// </summary>
    /// <param name="postalCode"></param>
    /// <returns>true if valid postal code, false if not</returns>
    public async Task<bool> ValidatePostalCode(int postalCode)
    {
        /// response content will be '[]' if invalid postal code
        var response = await supabaseClient.From<PostalCode>().Where(x => x.Code == postalCode).Get();
        return response?.Content?.Length != 2;
    }

    public async Task<List<Symptom>> GetSymptomsList()
    {
        var response = await supabaseClient.From<Symptom>().Where(x => true).Get();
        return response.Models;
    }
}