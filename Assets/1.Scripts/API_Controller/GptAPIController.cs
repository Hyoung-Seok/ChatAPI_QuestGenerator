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
    [Header("Prefabs")] 
    [SerializeField] private GameObject questionText;
    [SerializeField] private GameObject responseText;
    
    [Header("Component")] 
    [SerializeField] private Transform textParent;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;

    private OpenAIAPI _api;
    private Conversation _chat;
    
    private void Start()
    {
        InitGpt();
        SettingRole();

        sendButton.onClick.AddListener(StartConversation);
    }

    private async void StartConversation()
    {
        sendButton.interactable = false;
        
        var sendMsg = inputField.text;
        inputField.text = string.Empty;
        CreateTextBox(questionText, sendMsg);
        
        _chat.AppendUserInput(sendMsg);
        
        var response = await _chat.GetResponseFromChatbotAsync();
        CreateTextBox(responseText, response);
        
        sendButton.interactable = true;
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

    private void CreateTextBox(GameObject obj, string msg)
    {
        var textBox = Instantiate(obj, textParent);

        if (textBox.TryGetComponent(out TMP_Text text) == true)
        {
            text.text = msg;
        }
    }
}
