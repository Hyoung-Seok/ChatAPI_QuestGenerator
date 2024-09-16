using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Data", menuName = "Scriptable Object/Quest Data", order = int.MaxValue)]
public class QuestData : ScriptableObject
{
    [SerializeField] private string questID;
    [SerializeField] private string title;
    [SerializeField] private EQuestType questType;
    [SerializeField] private string npcName;
    [SerializeField] private string target;
    [SerializeField] private int count;
    [SerializeField] private List<string> scripts;

    public string QuestID => questID;
    public string Title => title;
    public EQuestType QuestType => questType;
    public string NpcName => npcName;
    public string Target => target;
    public int Count => count;
    public List<string> Scripts => scripts;
    
    public void InitQuestData(List<string> valueList)
    {
        scripts = new List<string>();

        questID = valueList[0];
        title = valueList[1];
        questType = ConvertQuestType(valueList[2]);
        npcName = valueList[3];
        target = valueList[4];
        count = int.Parse(valueList[5]);

        var texts = valueList[6].Split('*');
        foreach (var text in texts)
        {
            scripts.Add(text);
        }
    }

    public void OnClickEvent()
    {
        QuestManager.Instance.InitScripts(scripts);
    }
    
    private EQuestType ConvertQuestType(string type)
    {
        if (string.Equals(type, "Fight") == true)
        {
            return EQuestType.Fight;
        }
        
        if (string.Equals(type, "Get") == true)
        {
            return EQuestType.Get;
        }
        
        return EQuestType.Deliver;
    }
}

public enum EQuestType
{
    Fight = 0,
    Get = 1,
    Deliver = 2
}


