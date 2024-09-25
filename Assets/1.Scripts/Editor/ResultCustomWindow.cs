using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ResultCustomWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    private static Label _resultMessage;
    private static Label _processResult;

    public static string GetCurrentMessage => _resultMessage.text;

    public static void ShowResultWindow()
    {
        var window = GetWindow<ResultCustomWindow>();
        window.titleContent = new GUIContent("Result");
    }

    public static void CloseWindow()
    {
        _processResult.text = _resultMessage.text = string.Empty;
        
        var window = GetWindow<ResultCustomWindow>();
        window.Close();
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);

        _resultMessage = rootVisualElement.Q<Label>("ResultMessage");
        _processResult = rootVisualElement.Q<Label>("ProcessResult");
    }

    public static void UpdateMessage(string msg)
    {
        _resultMessage.text = msg;
    }
    
    public static void UpdateProcessMessage(string msg)
    {
        _processResult.text = msg;
    }
}
