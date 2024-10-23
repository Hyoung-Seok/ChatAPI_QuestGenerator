using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [Header("Quest")]
    [SerializeField] private List<QuestData> _questDatas;

    private bool _isInteraction = false;
    private PlayerController _playerController;
    private IEnumerator _headWeight;
    private WaitForEndOfFrame _waitForEndOfFrame;

    private void Start()
    {
        _playerController = GameManager.Instance.Player;

        _waitForEndOfFrame = new WaitForEndOfFrame();
    }

    public override void OnTriggerEnterEvent()
    {
        _isInteraction = true;
    }
    
    public override void OnTriggerStayEvent() { }

    public override void OnTriggerExitEvent()
    {
        _isInteraction = false;
    }

    private void OnInteractionStart()
    {
        npcCamera.gameObject.SetActive(true);
        
        StartHeadWeightRoutine(1);
    }

    private void OnInteractionEnd()
    {
        npcCamera.gameObject.SetActive(false);
        
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
    }
}
