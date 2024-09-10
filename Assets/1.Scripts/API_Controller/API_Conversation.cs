using OpenAI_API.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// OpenAPI Question & Response 클래스
/// SystemMessage에 입력된 정보에 따라 사용자 답변에 대답
/// </summary>
public class API_Conversation : OpenAIController
{
    [Header("Prefabs")] 
    [SerializeField] private GameObject questionText;
    [SerializeField] private GameObject responseText;
    
    [Header("Component")] 
    [SerializeField] private Transform textParent;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    
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

    protected override void InitGpt()
    {
        base.InitGpt();
        
        _chat = Api.Chat.CreateConversation();
        _chat.Model = AI_Model;
        _chat.RequestParameters.Temperature = 0;
    }

    private void SettingRole()
    {
        _chat.AppendSystemMessage("너는 유저에게 기업의 CEO를 알려주는 시스템이야." +
                                  "대답은 CEO이름 : , CEO나이 : , 회사설립일 : 로 해줘. " +
                                  "만약 기업 이름이 아니라 다른 질문이 들어오면 응답할 수 없습니다" +
                                  "라고 대답해.");
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
