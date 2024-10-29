using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EQuestState
{
    Start,
    Processing,
    Completion
}
public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<QuestData> curProcessQuest;
    [SerializeField] private GameObject curQuestPanel;
    [SerializeField] private Transform questPanelParent;

    private List<QuestDisplay> _curQuestDisplay;

    public void Init()
    {
        curProcessQuest = new List<QuestData>();
        _curQuestDisplay = new List<QuestDisplay>();
        
        EnemyBaseController.OnQuestUpdate += CheckEnemy;
    }

    public void AddQuest(QuestData data)
    {
        curProcessQuest.Add(data);

        var obj = Instantiate(curQuestPanel, questPanelParent).GetComponent<QuestDisplay>();
        obj.UpdateQuestDisplay(data.Title, data.TargetInfos);
        
        _curQuestDisplay.Add(obj);
    }

    public void RemoveQuestData(string key)
    {
        var index = 0;

        for (index = 0; index < curProcessQuest.Count; ++index)
        {
            if (string.Equals(curProcessQuest[index].Title, key) == true)
            {
                curProcessQuest.RemoveAt(index);
            }
        }

        var obj = _curQuestDisplay[index];
        Destroy(obj);
        _curQuestDisplay.RemoveAt(index);
    }

    private void CheckEnemy(string enemyName)
    {
        if (curProcessQuest.Count <= 0)
        {
            return;
        }

        var index = 0;
        foreach (var quest in curProcessQuest)
        {
            foreach (var target in quest.TargetInfos)
            {
                if (string.Equals(enemyName, target.TargetName) == false) continue;
                if (quest.CurQuestState == EQuestState.Completion) continue;
                
                target.CurTargetCount++;
                _curQuestDisplay[index].UpdateQuestDisplay(quest.TargetInfos);
                
                if (target.CurTargetCount == target.TargetCount)
                {
                    quest.CurQuestState = EQuestState.Completion;
                }
            }
            
            index++;
        }
    }
}
