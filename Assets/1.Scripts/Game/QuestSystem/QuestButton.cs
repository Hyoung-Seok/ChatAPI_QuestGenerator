using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private TextMeshProUGUI questStateText;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private Image questStateImg;

    public QuestData QuestData { get; private set; }
    private Button _button;

    public void SetQuestData(QuestData data)
    {
        QuestData = data;
        gameObject.GetComponent<Button>().onClick.AddListener(QuestClickEvent);
        
        questStateImg.sprite =
            GameManager.Instance.UIManager.GetQuestStateSprite(QuestData.CurQuestState);
        QuestStateTextUpdate(QuestData.CurQuestState);
        
        questTitleText.text = QuestData.Title;
    }

    public void UpdateQuestState(EQuestState state)
    {
        QuestData.CurQuestState = state;
        
        questStateImg.sprite =
            GameManager.Instance.UIManager.GetQuestStateSprite(QuestData.CurQuestState);

        QuestStateTextUpdate(state);
    }
    
    public void ResetQuestData()
    {
        questStateText.text = string.Empty;
        questTitleText.text = string.Empty;
        
        questStateImg.sprite = null;
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void QuestClickEvent()
    {
        GameManager.Instance.UIManager.OnQuestClickEvent(transform.GetSiblingIndex());
    }

    private void QuestStateTextUpdate(EQuestState state)
    {
        questStateText.text = state switch
        {
            EQuestState.Start => "<시작 가능>",
            EQuestState.Processing => "<진행중>",
            EQuestState.Completion => "<완료가능>",
            _ => " "
        };
    }
}
