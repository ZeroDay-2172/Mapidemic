using Google.GenAI;

namespace Mapidemic.Models;

public class Gemini
{
    private readonly string apiKey = "AIzaSyAY1K4JAEzU3b2uiQtQUX33oo5h2gqsgLE";
    private readonly string queryHeader = "I have the following symptoms: ";
    private readonly string firstRequest = "In ONE word, tell me the illness most closely represented by these symptoms.";
    private readonly string secondRequest = "In ONE number, one a new line tell me the average contagious period from the CDC for this illness in hours.";
    private readonly string thirdRequest = "In ONE number, on a new line tell me the average recover time from the CDC for this illness in hours.";
    private readonly string fourRequest = "On a new line, provide me with a full comma delimited list of symptoms for this illness as provided by the CDC.";
    private readonly Client gemini;

    /// <summary>
    /// The designated constructor for Gemini
    /// </summary>
    public Gemini ()
    {
        gemini = new Client(apiKey: apiKey);
    }

    /// <summary>
    /// A function that queries Google Gemini to check the symptoms
    /// of a user and get the most likely illness, its contagious period,
    /// recovery period, and full list of CDC symptoms
    /// </summary>
    /// <param name="symptoms"></param>
    /// <returns>the result string</returns>
    public async Task<string> AskGemini (string symptoms)
    {
        try // attempting to query Google Gemini
        {
            var response = await gemini.Models.GenerateContentAsync(
            model: "gemini-2.0-flash", 
            contents: $"{queryHeader}{symptoms}. {firstRequest} {secondRequest} {thirdRequest} {fourRequest}"
            );
            return response!.Candidates![0].Content!.Parts![0].Text!;
        }
        catch (Exception error) // error if Google Gemini could not be reached
        {
            return error.Message;
        }
    }
}