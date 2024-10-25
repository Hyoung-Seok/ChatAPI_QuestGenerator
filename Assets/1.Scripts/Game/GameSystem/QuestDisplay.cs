using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private TextMeshProUGUI questStateText;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private Image questStateImg;

    private QuestData _questData;
    private Button _button;

    public void SetQuestData(QuestData data)
    {
        _questData = data;
        gameObject.GetComponent<Button>().onClick.AddListener(QuestClickEvent);

        questTitleText.text = _questData.Title;
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
}
