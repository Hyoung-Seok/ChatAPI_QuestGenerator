using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ResultWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    private static Label _resultMessage;
    private static Label _processResult;

    public static string GetCurrentMessage => _resultMessage.text;
    
    private static bool _isOpen;

    public static void ShowResultWindow()
    {
        _isOpen = true;
        
        var window = GetWindow<ResultWindow>();
        window.titleContent = new GUIContent("Result");
    }

    public static void CloseWindow()
    {
        if(_isOpen == false) return;
        
        _processResult.text = _resultMessage.text = string.Empty;
        
        var window = GetWindow<ResultWindow>();
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

    private void OnDestroy()
    {
        _isOpen = false;
        QuestGeneratorManager.CloseAllWindow();
    }
}
