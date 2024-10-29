using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questCount;

    public void UpdateQuestDisplay(string title, List<TargetInfo> targets)
    {
        questTitle.text = title;
        questCount.text = string.Empty;

        foreach (var target in targets)
        {
            questCount.text +=
                $"{target.TargetName} : {target.CurTargetCount} / {target.TargetCount} \n";
        }
    }

    public void UpdateQuestDisplay(List<TargetInfo> targets)
    {
        questCount.text = string.Empty;
        
        foreach (var target in targets)
        {
            questCount.text +=
                $"{target.TargetName} : {target.CurTargetCount} / {target.TargetCount} \n";
        }
    }
    
    public void ResetQuestDisplay()
    {
        questTitle.text = string.Empty;
        questCount.text = string.Empty;
    }
}
