using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private TextMeshProUGUI questStateText;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private Image questStateImg;
    public event Action<int> OnClickAction;

    public void SetButtonData(QuestData data)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickAction?.Invoke(transform.GetSiblingIndex()));
        
        questStateImg.sprite = GameManager.Instance.QuestUIManager.GetQuestStateSprite(data.CurQuestState);
        QuestStateTextUpdate(data.CurQuestState);
        
        questTitleText.text = data.Title;
    }

    public void UpdateButtonState(EQuestState state)
    {
        questStateImg.sprite = GameManager.Instance.QuestUIManager.GetQuestStateSprite(state);
        QuestStateTextUpdate(state);
    }
    
    public void ResetButtonData()
    {
        questStateText.text = string.Empty;
        questTitleText.text = string.Empty;
        questStateImg.sprite = null;

        OnClickAction = null;
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
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
