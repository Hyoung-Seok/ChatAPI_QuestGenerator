using System;
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
    [SerializeField] private List<TargetInfo> targetInfo;

    [Header("Scripts")] 
    [SerializeField] private ScriptsData scriptsData;

    public EQuestState CurQuestState { get; set; }
    public string QuestID => questID;
    public string Title => title;
    public EQuestType QuestType => questType;
    public string NpcName => npcName;
    public List<TargetInfo> TargetInfos => targetInfo;
    public ScriptsData ScriptsData => scriptsData;
    
    public void InitQuestData(List<string> valueList)
    {
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
        
        scriptsData = new ScriptsData(valueList[6]);
        CurQuestState = EQuestState.Start;
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

[Serializable]
public class ScriptsData
{
    [Header("시작 대사")] 
    [SerializeField] private List<string> startScripts;

    [Header("수락 대사")] 
    [SerializeField] private string acceptScript;
    
    [Header("거절 대사")] 
    [SerializeField] private string refuseScript;
    
    [Header("진행중 대사")] 
    [SerializeField] private string processScript;
    
    public List<string> StartScripts => startScripts;
    public string AcceptScript => acceptScript;
    public string RefuseScript => refuseScript;
    public string ProcessScript => processScript;

    public ScriptsData(string json)
    {
        startScripts = new List<string>();

        var jObj = JObject.Parse(json);

        foreach (var kvp in jObj)
        {
            var value = kvp.Value.ToString();

            switch (kvp.Key)
            {
                case "OnStart":
                    startScripts.AddRange(value.Split('*'));
                    break;
                
                case "OnAccept":
                    acceptScript = value;
                    break;
                
                case "OnRefuse":
                    refuseScript = value;
                    break;
                
                case "OnProcess":
                    processScript = value;
                    break;
                
                default:
                    return;
            }
        }
    }
}

public enum EQuestType
{
    Fight = 0,
    Get = 1,
    Deliver = 2
}


