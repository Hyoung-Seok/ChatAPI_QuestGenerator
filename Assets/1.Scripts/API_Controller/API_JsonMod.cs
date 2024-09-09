using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;

public class API_JsonMod : OpenAIController
{
    [SerializeField] private TextMeshProUGUI resultText;
    
    private ChatRequest _chatRequest;
    
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

    public async void CreateJsonFile()
    {
        _chatRequest.Messages = new ChatMessage[]
        {
            new ChatMessage(ChatMessageRole.System, "너는 게임의 퀘스트에 대한 정보를 JSON으로 출력하도록 해" +
                                                    "사용자는 너에게 퀘스트를 주는 NPC의 기본적인 정보를 전달해줄거야." +
                                                    "이 정보를 바탕으로 게임의 퀘스트를 생성하고 이 NPC가 필요한 아이템이나," +
                                                    "잡아야 할 몬스터를 새롭게 생성해줘. 또한 이 퀘스트를 플레이어가 받을 때 나타날 대사도" +
                                                    "새로 생성해줘." +
                                                    "키 값은 Name(NPC의 이름 저장), Scripts(퀘스트를 줄 때 NPC의 대사), " +
                                                    "Item(필요한 아이템 없다면 NONE), Monster(잡아야 할 몬스터 없다면 NONE)으로 만들어줘."
                                                    ),
            new ChatMessage(ChatMessageRole.User, "NPC 이름은 잭슨. 큰 전투 후 심각한 부상을 입은 상태.")
        };

        var result = await Api.Chat.CreateChatCompletionAsync(_chatRequest);
        resultText.text = result.ToString();
    }
}

