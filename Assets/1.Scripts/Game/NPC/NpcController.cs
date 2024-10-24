using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [Header("QuestList")] 
    [SerializeField] private string idleScripts;
    [SerializeField] private List<QuestData> questList;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        QuestUIManager.Instance.SetInteractionButton(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) == true && QuestUIManager.Instance.CurrentState == false)
        {
            QuestUIManager.Instance.InitNpcTextInfo("찰리 중사", idleScripts);
            QuestUIManager.Instance.InitQuestButton(questList);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        QuestUIManager.Instance.SetInteractionButton(false);
    }
}
