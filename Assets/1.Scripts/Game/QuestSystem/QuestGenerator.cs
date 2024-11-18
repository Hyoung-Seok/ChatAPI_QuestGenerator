using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using File = System.IO.File;
using UnityEngine;

public class QuestGenerator
{
    private OpenAIAPI _api;
    private Model _model;
    private ChatRequest _chat;

    private string _systemMessage;

    private static string FILE_PATH = System.IO.Path.Combine(Application.streamingAssetsPath, "Prompt.txt");
    
    public QuestGenerator(Model model, float temperature)
    {
        _api = new OpenAIAPI(KeyEncryption.DecodingBase64());
        _chat = new ChatRequest()
        {
            Model = model,
            Temperature = temperature,
            ResponseFormat = ChatRequest.ResponseFormats.JsonObject
        };

        _systemMessage = File.ReadAllText(FILE_PATH);
    }

    public async Task<string> CreateJsonMessage(string chatMsg)
    {
        _systemMessage = await File.ReadAllTextAsync(FILE_PATH);
        Debug.Log(_chat.Temperature);
        
        _chat.Messages = new[]
        {
            new ChatMessage(ChatMessageRole.System, _systemMessage),
            new ChatMessage(ChatMessageRole.User, chatMsg)
        };
        
        var result = await _api.Chat.CreateChatCompletionAsync(_chat);
        TokensData.SaveTokenData(result.Usage);
        
        return result.ToString();
    }

    public void ChangeTemperature(float temp)
    {
        if (0 > temp && 1 < temp)
        {
            return;
        }

        _chat.Temperature = temp;
    }
}
