using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class QuestGeneratorManager : EditorWindow
{
    [Header("Data")] 
    public static string NpcData;
    
    [MenuItem("OpenAI/QuestGenerator")]
    private static void CreateWindow()
    {
        ResultCustomWindow.ShowResultWindow();
        NpcDataUI.CreateWindow();
    }

    public void CreateGUI()
    {
        
    }

    public static string GetFullFilePath(string path, string name)
    {
        var result = Path.Combine(Application.dataPath, path);
        result = Path.Combine(result, name + ".xlsx");

        return result;
    }
}
