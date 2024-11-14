using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class QuestScriptableGenerator
{
    public static void CreateAndSaveScriptableObj<T>(List<string> data) where T : ScriptableObject
    {
        var sObj = ScriptableObject.CreateInstance<T>();

        if (sObj is QuestData questData)
        {
            questData.InitQuestData(data);
        }

        var path = "Assets/_QuestData/" + data[0] + ".asset";
        
        AssetDatabase.CreateAsset(sObj, path);
        AssetDatabase.SaveAssets();
    }
    
    public static void CreateAndSaveScriptableObj<T>(List<List<string>> data) where T : ScriptableObject
    {
        foreach (var valueList in data)
        {
            var sObj = ScriptableObject.CreateInstance<T>();
            
            if (sObj is QuestData questData)
            {
                questData.InitQuestData(valueList);
            }            
            
            var path = "Assets/_QuestData/" + valueList[0] + ".asset";
        
            AssetDatabase.CreateAsset(sObj, path);
            AssetDatabase.SaveAssets();
        }
    }
}
