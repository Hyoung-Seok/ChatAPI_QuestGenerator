using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class NpcController : Interactable
{
    [field: SerializeField] public string NpcName { get; private set; }
    
    [Header("Component")] 
    [SerializeField] private CinemachineVirtualCamera npcCamera;
    [SerializeField] private MultiAimConstraint headRig;
    
    [Header("Setting")] 
    [SerializeField] private float headWeightTime = 1.0f;
    [field: SerializeField] public string DefaultText { get; private set; }

    [Header("Quest")] 
    [SerializeField] private List<QuestContainer> questContainer;

    public event Action OnEnableQuestUIAction;
    public event Action OnDisableQuestUIAction;
    
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
    
    public List<QuestData> GetFirstQuestDataList()
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

    public void RemoveQuestData(QuestData data)
    {
        foreach (var container in questContainer)
        {
            if (container.QuestData.Contains(data) == false)
            {
                continue;
            }
            
            container.QuestData.Remove(data);

            if (container.QuestData.Count <= 0)
            {
                questContainer.Remove(container);
            }
            break;
        }
    }

    public QuestData GetNextQuestAndRemove(QuestData data)
    {
        QuestData result = null;
        
        foreach (var quest in questContainer)
        {
            if(quest.QuestData.Contains(data) == false) continue;

            var index = quest.QuestData.IndexOf(data);
            result = quest.QuestData[index + 1];
            quest.QuestData.RemoveAt(index + 1);
        }

        return result;
    }
    
    public void AddDeliverQuest(QuestData data)
    {
        data.CurQuestState = EQuestState.Completion;
        var container = new QuestContainer
        {
            QuestData = new List<QuestData> { data }
        };

        questContainer.Add(container);
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
       
                GameManager.Instance.QuestManager.UpdateQuestManagerData(this);
                GameManager.Instance.QuestPresenter.Init();
       
                OnEnableQuestUIAction?.Invoke();
                break;
            
            case false:
                OnDisableQuestUIAction?.Invoke();
                
                GameManager.Instance.QuestPresenter.Clear();
                GameManager.Instance.QuestManager.ResetQuestManagerData();
                
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
}
