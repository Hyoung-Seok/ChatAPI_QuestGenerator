using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questCount;

    public string Title => questTitle.text;

    public void UpdateQuestDisplay(string title, List<TargetInfo> targets, EQuestType type)
    {
        questTitle.text = title;
        questCount.text = string.Empty;

        switch (type)
        {
            case EQuestType.Fight:
            case EQuestType.Get:
                foreach (var target in targets)
                {
                    questCount.text +=
                        $"{target.TargetName} : {target.CurTargetCount} / {target.TargetCount} \n";
                }
                break;
            
            case EQuestType.Deliver:
                foreach (var target in targets)
                {
                    questCount.text +=
                        $"{target.TargetName} 에게 물건 전해주기. \n";
                }
                break;
            
            default:
                return;
        }
    }

    public void UpdateQuestDisplay(QuestData data)
    {
        questCount.text = string.Empty;
        
        switch (data.QuestType)
        {
            case EQuestType.Fight:
            case EQuestType.Get:
                foreach (var target in data.TargetInfos)
                {
                    questCount.text +=
                        $"{target.TargetName} : {target.CurTargetCount} / {target.TargetCount} \n";
                }
                break;
            
            case EQuestType.Deliver:
                foreach (var target in data.TargetInfos)
                {
                    questCount.text +=
                        $"{target.TargetName} 에게 물건 전해주기. \n";
                }
                break;
            
            default:
                return;
        }
    }
}
