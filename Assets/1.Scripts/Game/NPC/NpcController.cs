using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NpcController : Interactable
{
    [Header("Component")] 
    [SerializeField] private CinemachineVirtualCamera npcCamera;
    [SerializeField] private MultiAimConstraint headRig;

    [Header("Setting")] 
    [SerializeField] private float headWeightTime = 1.0f;
    [SerializeField] private string defaultText;

    [Header("Quest")] 
    [SerializeField] private List<QuestContainer> questContainer;

    [field: SerializeField]public string NpcName { get; private set; }
    private PlayerController _playerController;
    private IEnumerator _headWeight;
    private WaitForEndOfFrame _waitForEndOfFrame;

    private void Start()
    {
        _playerController = GameManager.Instance.Player;
        _waitForEndOfFrame = new WaitForEndOfFrame();

        if (questContainer.Count > 0)
        {
            NpcName = questContainer[0].QuestData[0].NpcName;
        }
        
        GameManager.Instance.NpcManager.AddNpcControllerInDictionary(NpcName, this);
    }

    protected override void OnTriggerEnterEvent()
    {
        GameManager.Instance.NpcManager.EnterInteraction += OnInteractionStart;
        GameManager.Instance.NpcManager.ExitInteraction += OnInteractionEnd;
    }
    
    protected override void OnTriggerStayEvent() { }

    protected override void OnTriggerExitEvent()
    {
        GameManager.Instance.NpcManager.EnterInteraction -= OnInteractionStart;
        GameManager.Instance.NpcManager.ExitInteraction -= OnInteractionEnd;
    }

    public void RemoveQuestData(int index)
    {
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
            
        GameManager.Instance.UIManager.UpdateQuestPanel(curQuest);
    }

    private void OnInteractionStart()
    {
        npcCamera.gameObject.SetActive(true);
        _playerController.ChangeMainState(_playerController.InteractionState);
        
        StartHeadWeightRoutine(1);
    }

    private void OnInteractionEnd()
    {
        if (npcCamera.gameObject.activeSelf == false)
        {
            return;
        }
        
        npcCamera.gameObject.SetActive(false);
        _playerController.ChangeMainState(_playerController.MoveState);
        StartHeadWeightRoutine(0);
    }

    private void StartHeadWeightRoutine(float target)
    {
        if (_headWeight != null)
        {
            StopCoroutine(_headWeight);
        }
        
        _headWeight = HeadWeight(target);
        StartCoroutine(_headWeight);
    }

    private IEnumerator HeadWeight(float target)
    {
        var curTime = 0.0f;

        while (curTime < headWeightTime)
        {
            curTime += Time.deltaTime;
            headRig.weight = Mathf.Lerp(headRig.weight, target, curTime / headWeightTime);

            yield return _waitForEndOfFrame;
        }

        headRig.weight = target;
        if (target <= 0)
        {
            GameManager.Instance.UIManager.DisableNpcUI();
        }
        else
        {
            var curQuest = new List<QuestData>();
            foreach (var quest in questContainer)
            {
                if (quest.QuestData.Count != 0)
                {
                    curQuest.Add(quest.QuestData[0]);
                }
            }
            
            GameManager.Instance.UIManager.UpdateDefaultText(defaultText);
            GameManager.Instance.UIManager.EnableNpcUI(curQuest, NpcName);
        }
    }
}
