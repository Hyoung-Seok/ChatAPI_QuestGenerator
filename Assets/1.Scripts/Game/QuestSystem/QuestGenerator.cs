using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

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

        _systemMessage = "너는 게임의 퀘스트를 JSON형식으로 생성하는 어시스던트야. 아래와 같은 형태로 JSON을 생성해줘." +
                         "QuestID : 퀘스트의 ID. NPC의 첫 알파벳 + 퀘스트 타입의 첫 알파벳 + 1~9999 사이에 랜덤한 숫자로 지정." +
                         "Title : 퀘스트의 제목." +
                         "Type : 퀘스트 타입. 전투(Fight), 얻어오기(Get), 전달하기(Deliver), 완료(Finish)가 있어. 퀘스트 타입에 맞춰서 스크립트를 생성해줘." +
                         "만약 퀘스트의 타입이 Deliver(전달하기)라면, 전해주는 대상한테 퀘스트를 완료할 수 있도록 퀘스트를 하나 더 만들어줘." +
                         "전달하기 타입의 퀘스트 후 퀘스트가 더 이상 연계되지 않는다면, 하나 더 만드는 퀘스트 타입은 Finish(완료). " +
                         "이 퀘스트는 단순 완료용 퀘스트이므로, 스크립트에 다른 목표를 만들지 마." +
                         "NpcName : 퀘스트를 주는 NPC의 이름" +
                         "Target : 잡아야 할 몬스터, 얻어야 할 아이템, 전달 목표인 NPC이름. 여러개의 객체 이름이 넘어왔을 때 각 객체는 /로 분리해줘." +
                         "Count : 갯수. Target이 여러개라면 Count도 동일하게 /로 각각 잡아야 할 숫자를 분리해줘. " +
                         "Scripts : 퀘스트를 줄 때의 NPC의 대사. 플레이어가 퀘스트를 받을 때 나올 대사와, 수락할 때의 대사, 퀘스트를 완료했을 때 나타날 대사로 만들어줘." +
                         "대화의 종류를 구분하기 위해 각 대사 사이에는 *를 넣어주고 다른 키값은 넣지 마. 대사는 최소 3줄 이상 작성해줘. 만약 퀘스트에서 스토리를 말해줘야 한다면" +
                         "플레이어가 퀘스트를 보고 스토리를 파악할 수 있게 작성해줘." +
                         "연계되는 퀘스트의 생성을 요청하면 각 퀘스트를 Quest1,Quest2로 묶어줘." +
                         "Key, Value가 완성된 형태로 전달된다면 전달된 퀘스트와 연계되는 퀘스트를 생성해줘. 이 때 기존에 전달된 퀘스트는 반환하지마.";
    }

    public async Task<string> CreateJsonMessage(string chatMsg)
    {
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
