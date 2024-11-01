using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NpcController : Interactable
{
    [field: SerializeField] public string NpcName { get; private set; }
    
    [Header("Component")] 
    [SerializeField] private CinemachineVirtualCamera npcCamera;
    [SerializeField] private MultiAimConstraint headRig;
    
    [Header("Setting")] 
    [SerializeField] private float headWeightTime = 1.0f;
    [SerializeField] private string defaultText;

    [Header("Quest")] 
    [SerializeField] private List<QuestContainer> questContainer;
    
    private void Start()
    {
        if (questContainer.Count > 0)
        {
            NpcName = questContainer[0].QuestData[0].NpcName;
        }
        
        GameManager.Instance.NpcManager.AddNpcControllerInDictionary(NpcName, this);
    }

    protected override void OnTriggerEnterEvent()
    {
        GameManager.Instance.NpcManager.InteractionEvent += OnInteractionEvent;
    }
    
    protected override void OnTriggerStayEvent() { }

    protected override void OnTriggerExitEvent()
    {
        GameManager.Instance.NpcManager.InteractionEvent -= OnInteractionEvent;
    }

    public void RemoveQuestData(int index)
    {
        if (questContainer[index].QuestData[0].QuestType == EQuestType.Deliver)
        {
            var pair = questContainer[index].QuestData[0].ChainQuest;
            GameManager.Instance.NpcManager.GetNpcControllerOrNull(pair.Key)?.RemoveDeliverQuest(pair.Value);
        }
        
        questContainer[index].QuestData.RemoveAt(0);

        if (questContainer[index].QuestData.Count <= 0)
        {
            questContainer.RemoveAt(index);
            return;
        }
        
        var curQuest = new List<QuestData>();
        foreach (var quest in questContainer)
        {
            curQuest.Add(quest.QuestData[0]);   
        }
            
        GameManager.Instance.QuestUIManager.UpdateQuestPanel(curQuest);
    }

    public void SetDeliverQuestData(QuestData data)
    {
        var target = GameManager.Instance.NpcManager.GetNpcControllerOrNull(data.TargetInfos[0].TargetName);
        int num;
        var index = -1;

        for (num = 0; num < questContainer.Count; ++num)
        {
            index = questContainer[num].QuestData.IndexOf(data);
            if (index >= 0 && index <= questContainer[num].QuestData.Count - 1)
            {
                break;
            }
        }
        
        target.AddDeliverQuest(questContainer[num].QuestData[index + 1]);
        
        questContainer[num].QuestData[index + 1].AddChainQuest(NpcName, questContainer[num].QuestData[index]);
        questContainer[num].QuestData.RemoveAt(index + 1);
    }

    private void AddDeliverQuest(QuestData data)
    {
        data.CurQuestState = EQuestState.Completion;
        var container = new QuestContainer
        {
            QuestData = new List<QuestData> { data }
        };

        questContainer.Add(container);
    }

    private void RemoveDeliverQuest(QuestData data)
    {
        foreach (var container in questContainer)
        {
            var index = container.QuestData.IndexOf(data);
            if (index <= -1)
            {
                continue;
            }
            
            container.QuestData.RemoveAt(index);
            GameManager.Instance.QuestUIManager.DeregisterProcessQuest(data.Title);
        }
    }

    private void OnInteractionEvent(bool enable)
    {
        if (enable == true)
        {
            npcCamera.gameObject.SetActive(true);
            GameManager.Instance.ChangePlayerState("Interaction");
            
            OnInteractionStartRoutine(true).Forget();   
        }
        else
        {
            if (npcCamera.gameObject.activeSelf == false)
            {
                return;
            }
            
            GameManager.Instance.ChangePlayerState("Move");
            OnInteractionStartRoutine(false).Forget();
        }
    }
    
    private async UniTask OnInteractionStartRoutine(bool enable)
    {
        var curTime = 0.0f;
        npcCamera.gameObject.SetActive(enable);
        
        switch (enable)
        {
            case true:
                while (curTime < headWeightTime)
                {
                    curTime += Time.deltaTime;
                    headRig.weight = Mathf.Lerp(headRig.weight, 1, curTime / headWeightTime);

                    await UniTask.WaitForEndOfFrame(this);
                }

                headRig.weight = 1;
                
                GameManager.Instance.QuestUIManager.EnableQuestDisplay(GetFirstQuestDataList(), NpcName, defaultText);
                GameManager.Instance.QuestManager.CurInteractionNpc = this;
                break;
            
            case false:
                GameManager.Instance.QuestUIManager.DisableQuestDisplay();
                GameManager.Instance.QuestManager.CurInteractionNpc = null;
                
                while (curTime < headWeightTime)
                {
                    curTime += Time.deltaTime;
                    headRig.weight = Mathf.Lerp(headRig.weight, 1, curTime / headWeightTime);

                    await UniTask.WaitForEndOfFrame(this);
                }
                headRig.weight = 0;
                break;
        }
    }

    private List<QuestData> GetFirstQuestDataList()
    {
        var questList = new List<QuestData>();

        foreach (var container in questContainer)
        {
            if (container.QuestData.Count != 0)
            {
                questList.Add(container.QuestData[0]);
            }
        }

        return questList;
    }
}
