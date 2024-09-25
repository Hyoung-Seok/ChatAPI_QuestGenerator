using System.IO;
using UnityEditor;
using UnityEngine;

public class QuestGeneratorManager : EditorWindow
{
    [Header("Data")] 
    public static string NpcData;
    public static string OtherData;
    public static string ResultData;
    
    [MenuItem("OpenAI/QuestGenerator")]
    private static void CreateWindow()
    {
        ResultCustomWindow.ShowResultWindow();
        
        NpcDataUI.CreateWindow();
        OtherDataUI.CreateWindow();
        ControlPanel.CreateWindow();
    }

    public void CreateGUI()
    {
        
    }

    public static string GetFullFilePath(string path, string name)
    {
        var result = Path.Combine(Application.dataPath, path);

        if (name.Contains(".xlsx") == false)
        {
            name += ".xlsx";
        }
        
        result = Path.Combine(result, name);

        return result;
    }

    public static void CloseAllWindow()
    {
        NpcDataUI.CloseWindow();
        OtherDataUI.CloseWindow();
        ControlPanel.CloseWindow();
        ResultCustomWindow.CloseWindow();
    }
}
