using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using UnityEngine;

public enum EChatModel
{
    GPT4,
    GPT4_TERBO,
    CHAT_GPT_TURBO,
    CHAT_GPT_16K
}

public class QuestGenerator
{
    private OpenAIAPI _api;
    private Model _model;
    private ChatRequest _chat;

    private string _systemMessage;
    
    public QuestGenerator(EChatModel model, float temperature)
    {
        _api = new OpenAIAPI(KeyEncryption.DecodingBase64());
        _chat = new ChatRequest()
        {
            Model = GetAPIModel(model),
            Temperature = temperature,
            ResponseFormat = ChatRequest.ResponseFormats.JsonObject
        };
        
        _systemMessage = "너는 게임의 퀘스트를 생성하는 어시스던트야." +
                         "기본적인 정보를 입력받아 퀘스트를 생성할 수 있도록 JSON형식으로 출력하도록 해." +
                         "기본적인 Key값은 다음과 같아." +
                         "QuestID : 퀘스트의 아이디. 아이디는 퀘스트 타입에 앞글자 + 퀘스트를 주는 NPC의 앞글자 스팰링 + 0001~9999중 랜덤으로 조합해서 생성해줘." +
                         "예를 들어 퀘스트 타입이 Get(얻어오기) 이고 NPC의 이름이 미첼이라면 GM0542이런 식으로 생성해줘." +
                         "Title : 퀘스트의 제목, " +
                         "Type : 퀘스트의 타입. 이 타입에는 Fight(전투), Get(얻어오기), Deliver(전달하기)가 있어. 이 퀘스트 타입은 사용자가 알려줄거야." +
                         "NpcName : 이 퀘스트를 주는 NPC의 이름." +
                         "Target : 잡아야 할 몬스터, 얻어야 할 아이템, 전달 목표인 npc이름이 들어가." +
                         "Count : 갯수" +
                         "Scripts : 퀘스트를 줄 때 NPC의 대사. 대사는 플레이어가 퀘스트를 받을 때 나타나는 대사와 퀘스트를 수락하고 나타나는 대사를 만들어줘." +
                         "이 두 종류의 대사를 구분하기 위해 대사 사이에는 *을 넣어주고 다른 키 값은 생성하지 마.";
    }

    public async Task<string> CreateJsonMessage(string chatMsg)
    {
        _chat.Messages = new[]
        {
            new ChatMessage(ChatMessageRole.System, _systemMessage),
            new ChatMessage(ChatMessageRole.User, chatMsg)
        };

        var result = await _api.Chat.CreateChatCompletionAsync(_chat);
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

    public void ChangeModel(EChatModel mode)
    {
        _chat.Model = GetAPIModel(mode);
    }

    public void ChangeSystemMessage(string msg)
    {
        _systemMessage = msg;
    }
    
    private Model GetAPIModel(EChatModel model)
    {
        return model switch
        {
            EChatModel.GPT4 => Model.GPT4,
            EChatModel.GPT4_TERBO => Model.GPT4_Turbo,
            EChatModel.CHAT_GPT_TURBO => Model.ChatGPTTurbo_16k,
            EChatModel.CHAT_GPT_16K => Model.ChatGPTTurbo_16k,
            _ => Model.GPT4
        };
    }
}
