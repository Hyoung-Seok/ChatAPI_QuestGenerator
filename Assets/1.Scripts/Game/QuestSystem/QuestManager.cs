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
    [Header("Data")]
    [SerializeField] private List<QuestData> curProcessQuest;
    [SerializeField] private List<QuestDisplay> curQuestDisplay;
    [field: SerializeField] public NpcController CurInteractionNpc { get; set; }

    public List<QuestData> CurrentProcessQuest => curProcessQuest;
    public List<QuestDisplay> CurQuestDisplay => curQuestDisplay;
    
    public void Init()
    {
        curProcessQuest = new List<QuestData>();
        curQuestDisplay = new List<QuestDisplay>();
        
        EnemyBaseController.OnQuestUpdate += CheckEnemy;
    }

    public void RemoveClearQuest(int index, QuestData data)
    {
        curProcessQuest.Remove(data);
        CurInteractionNpc.RemoveQuestData(index);
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
                curQuestDisplay[index].UpdateQuestDisplay(quest.TargetInfos, quest.QuestType);
                
                if (target.CurTargetCount == target.TargetCount)
                {
                    quest.CurQuestState = EQuestState.Completion;
                }
            }
            
            index++;
        }
    }
}
