using OpenAI;
using OpenAI.Chat;

namespace Mapidemic.Models;

public class OpenAIService
{
    // chat message constants
    private const string MessageHeader = "I have the following symptoms: ";
    private const string MessageFooter = ". In one word, tell me the illness that I am most likely to have.";
    // instance variables
    private const string OpenAiVersion = "gpt-5";
    private const string SecretKey = "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";
    private readonly OpenAIClient openAIClient;

    /// <summary>
    /// The designated constructor for an OpenAIService
    /// </summary>
    public OpenAIService()
    {
        openAIClient = new OpenAIClient(SecretKey);
    }

    /// <summary>
    /// A function that asks chat-gpt for a likely illness
    /// based on the users list of symptoms
    /// </summary>
    /// <param name="symptomString"></param>
    /// <returns>the likely illness predicted by chat-gpt</returns>
    public async Task<string> GetLikelyIllness(string symptomString)
    {
        var chat = openAIClient.GetChatClient(OpenAiVersion);
        var response = await chat.CompleteChatAsync([
            ChatMessage.CreateUserMessage($"{MessageHeader}{symptomString}{MessageFooter}")
        ]);
        return response.Value.Content[0].Text;
    }
}