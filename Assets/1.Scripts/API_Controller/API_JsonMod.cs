using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;

public class API_JsonMod : OpenAIController
{
    [Header("Component")]
    [SerializeField] private TextMeshProUGUI resultText;

    public string Result => resultText.text;
    
    private ChatRequest _chatRequest;
    private string _systemMessage;
    
    private void Start()
    {
        InitGpt();
    }

    protected override void InitGpt()
    {
        base.InitGpt();

        _chatRequest = new ChatRequest()
        {
            Model = AI_Model,
            Temperature = 0.0,
            ResponseFormat = ChatRequest.ResponseFormats.JsonObject
        };
    }

    public async void CreateJsonMessage(string chatMessage)
    {
        _chatRequest.Messages = new[]
        {
            new ChatMessage(ChatMessageRole.System, _systemMessage),
            new ChatMessage(ChatMessageRole.User, chatMessage)
        };
        
        var result = await Api.Chat.CreateChatCompletionAsync(_chatRequest);
        resultText.text = result.ToString();
    }

    public void InitSystemMessage(string msg)
    {
        _systemMessage = msg;
    }
}

