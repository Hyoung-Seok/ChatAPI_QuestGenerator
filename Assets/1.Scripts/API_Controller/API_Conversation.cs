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
