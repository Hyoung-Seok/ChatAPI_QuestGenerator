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

    public void Init()
    {
        curProcessQuest = new List<QuestData>();
        EnemyBaseController.OnQuestUpdate += CheckEnemy;
    }

    public void AddQuest(QuestData data)
    {
        curProcessQuest.Add(data);
    }

    private void CheckEnemy(string enemyName)
    {
        if (curProcessQuest.Count <= 0)
        {
            return;
        }
        
        foreach (var quest in curProcessQuest)
        {
            foreach (var target in quest.TargetInfos)
            {
                if (string.Equals(enemyName, target.TargetName) == false) continue;
                if (quest.CurQuestState == EQuestState.Completion) continue;
                
                target.CurTargetCount++;
                if (target.CurTargetCount == target.TargetCount)
                {
                    quest.CurQuestState = EQuestState.Completion;
                }
            }
        }
    }
}
