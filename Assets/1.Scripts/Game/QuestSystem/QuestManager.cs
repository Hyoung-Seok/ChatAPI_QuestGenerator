using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
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
    [field: SerializeField] public List<QuestData> CurNpcQuest { get; private set; }
    [field: SerializeField] public NpcController CurInteractionNpc { get; private set; }
    
    public event Action<QuestData> UpdateProcessQuest;
    public event Action<QuestData, int> UpdateItemProcessQuest; 
    
    public void Init()
    {
        curProcessQuest = new List<QuestData>();
        EnemyBaseController.OnQuestUpdate += CheckEnemy;
    }

    public void UpdateQuestManagerData(NpcController npcController)
    {
        CurInteractionNpc = npcController;
        CurNpcQuest = CurInteractionNpc.GetFirstQuestDataList();
    }

    public void ResetQuestManagerData()
    {
        CurInteractionNpc = null;
        CurNpcQuest = null;
    }
    
    public bool RemoveClearQuest(QuestData data)
    {
        if (curProcessQuest.Contains(data) == false)
        {
            return false;
        }

        curProcessQuest.Remove(data);
        CurInteractionNpc.RemoveQuestData(data);
        
        CurNpcQuest = CurInteractionNpc.GetFirstQuestDataList();
        return true;
    }
    
    public void UpdateQuest(int index, EQuestState questState)
    {
        CurNpcQuest[index].CurQuestState = questState;
        curProcessQuest.Add(CurNpcQuest[index]);
    }

    public void SetDeliverQuest(int index)
    {
        var deliverQuestData = CurNpcQuest[index];
        var deliverFinishQuest = CurInteractionNpc.GetNextQuestAndRemove(deliverQuestData);
        
        // 체인 퀘스트 등록
        deliverFinishQuest.ChainQuest = new KeyValuePair<string, QuestData>(deliverQuestData.Title, deliverQuestData);
        curProcessQuest.Add(deliverFinishQuest);
        
        var target = GameManager.Instance.NpcManager.GetNpcControllerOrNull(deliverQuestData.TargetInfos[0].TargetName);
        target.AddDeliverQuest(deliverFinishQuest);
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
                var itemName = enemyName.Replace(" ", "");
                var targetName = target.TargetName.Replace(" ", "");
                
                if (string.Equals(itemName, targetName) == false) continue;
                if (quest.CurQuestState == EQuestState.Completion) continue;
                
                target.CurTargetCount++;
                UpdateProcessQuest?.Invoke(quest);
                
                if (target.CurTargetCount == target.TargetCount)
                {
                    quest.CurQuestState = EQuestState.Completion;
                }
            }
        }
    }

    public void CheckItem(ItemData itemData, int count)
    {
        if (curProcessQuest.Count <= 0)
        {
            return;
        }

        foreach (var questData in curProcessQuest)
        {
            foreach (var target in questData.TargetInfos)
            {
                var splitItemName = itemData.ItemName.Replace(" ", "");
                var targetName = target.TargetName.Replace(" ", "");
                
                if(string.Equals(splitItemName, targetName) == false) continue;

                target.CurTargetCount = count;
                UpdateItemProcessQuest?.Invoke(questData, count);
                
                if (target.CurTargetCount == target.TargetCount)
                {
                    questData.CurQuestState = EQuestState.Completion;
                }
            }
        }
    }
}
