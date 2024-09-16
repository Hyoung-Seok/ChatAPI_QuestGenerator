using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [Header("QuestList")] 
    [SerializeField] private string idleScripts;
    [SerializeField] private List<QuestData> questList;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) == true && QuestManager.Instance.CurrentState == false)
        {
            QuestManager.Instance.InitNpcTextInfo("찰리 중사", idleScripts);
            QuestManager.Instance.InitQuestButton(questList);
        }
    }
}
