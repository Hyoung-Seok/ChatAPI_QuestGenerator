using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X509;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Data", menuName = "Scriptable Object/Quest Data", order = int.MaxValue)]
public class QuestData : ScriptableObject
{
    [SerializeField] private string questID;
    [SerializeField] private string title;
    [SerializeField] private EQuestType questType;
    [SerializeField] private string npcName;
    [SerializeField] private List<TargetInfo> targetInfo;
    [SerializeField] private List<string> scripts;

    public string QuestID => questID;
    public string Title => title;
    public EQuestType QuestType => questType;
    public string NpcName => npcName;
    public List<TargetInfo> TargetInfos => targetInfo;
    public List<string> Scripts => scripts;
    
    public void InitQuestData(List<string> valueList)
    {
        scripts = new List<string>();
        targetInfo = new List<TargetInfo>();

        questID = valueList[0];
        title = valueList[1];
        questType = ConvertQuestType(valueList[2]);
        npcName = valueList[3];
        
        var target = valueList[4].Split('/');
        var count = valueList[5].Split('/');
        for (var i = 0; i < target.Length; ++i)
        {
            if (int.TryParse(count[i], out var res) == false)
            {
                break;
            }
            targetInfo.Add(new TargetInfo(target[i], res));    
        }
        
        var texts = valueList[6].Split('*');
        foreach (var text in texts)
        {
            scripts.Add(text);
        }
    }

    public void OnClickEvent()
    {
        QuestUIManager.Instance.InitScripts(scripts);
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

[Serializable]
public class TargetInfo
{
    [SerializeField] private string targetName;
    [SerializeField] private int targetCount;

    public string TargetName => targetName;
    public int TargetCount => targetCount;

    public TargetInfo(string name, int count)
    {
        targetName = name;
        targetCount = count;
    }
}

public enum EQuestType
{
    Fight = 0,
    Get = 1,
    Deliver = 2
}


