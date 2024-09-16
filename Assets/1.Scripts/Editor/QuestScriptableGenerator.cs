using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class QuestScriptableGenerator : EditorWindow
{
    private string _filePath = string.Empty;
    private int _row = 1;
    private ExcelReader _excelReader;
        
    [MenuItem("OpenAI/Generate Scriptable")]
    private static void Init()
    {
        var window = (QuestScriptableGenerator)GetWindow(typeof(QuestScriptableGenerator));
        
        window.Show();
        window.titleContent.text = "Quest Scriptable";
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        _filePath = Application.dataPath + "/_ExcelData/QuestTable.xlsx";
        _filePath = EditorGUILayout.TextField("File Path", _filePath);
        
        _row = EditorGUILayout.IntField("Row", _row);
        
        GUILayout.Space(30);
        
        if (GUILayout.Button("Generate"))
        {
            _excelReader = new ExcelReader(_filePath);
            var valueList = _excelReader.GetAllRowData(_row);
            
            CreateAndSaveScriptableObj<QuestData>(valueList);
        }
    }

    private static void CreateAndSaveScriptableObj<T>(List<string> data) where T : ScriptableObject
    {
        var sObj = ScriptableObject.CreateInstance<T>();

        if (sObj is QuestData questData)
        {
            questData.InitQuestData(data);
        }

        var path = "Assets/QuestData/" + data[0] + ".asset";
        
        AssetDatabase.CreateAsset(sObj, path);
        AssetDatabase.SaveAssets();
    }
}
