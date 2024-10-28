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
    }

    public void AddQuest(QuestData data)
    {
        _curProcessQuest.Add(data);
    }
}
