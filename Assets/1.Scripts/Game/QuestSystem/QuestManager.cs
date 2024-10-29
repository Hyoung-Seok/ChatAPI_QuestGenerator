using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EQuestState
{
    Start,
    Processing,
    Completion
}
public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<QuestData> _curProcessQuest;

    public void Init()
    {
        _curProcessQuest = new List<QuestData>();
        EnemyBaseController.OnQuestUpdate += CheckEnemy;
    }

    public void AddQuest(QuestData data)
    {
        _curProcessQuest.Add(data);
    }

    private void CheckEnemy(string enemyName)
    {
        if (_curProcessQuest.Count <= 0)
        {
            return;
        }
        
        foreach (var quest in _curProcessQuest)
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
