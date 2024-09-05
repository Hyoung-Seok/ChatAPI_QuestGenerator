using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// OpenAPI 사용 메서드는 Update에 작성하지 말것.
/// 프레임마다 통신을 시도해 AI 크레딧이 매우 많이 사용됨.
/// </summary>
public class GptAPIController : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private TMP_Text textField;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;

    private OpenAIAPI _api;
    private Conversation _chat;
    private List<ChatMessage> _messageList;
    
    private void Start()
    {
        InitGpt();
        SettingRole();

        sendButton.onClick.AddListener(() => StartConversation());
    }

    private async void StartConversation()
    {
        textField.text = string.Empty;
        
        var sendMsg = inputField.text;
        _chat.AppendUserInput(sendMsg);
        
        string response = await _chat.GetResponseFromChatbotAsync();
        
        Debug.Log(response);
        textField.text = "ChatGPT : " + response;
    }
    
    private void InitGpt()
    {
        _api = new OpenAIAPI(KeyEncryption.DecodingBase64());
        _chat = _api.Chat.CreateConversation();
        _chat.Model = Model.GPT4_Turbo;
        _chat.RequestParameters.Temperature = 0;
    }

    private void SettingRole()
    {
        _chat.AppendSystemMessage("사용자에 대답에 친절하게 응대해줘.");
    }
}
