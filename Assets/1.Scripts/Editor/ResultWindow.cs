using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ResultWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    
    private static Label _curNpcData;
    private static Label _curOtherData;
    private static Label _resultMessage;
    private static Label _processResult;
    
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
        
        var window = GetWindow<ResultWindow>();
        window.Close();
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        InitUI();
    }

    private void InitUI()
    {
        _processResult = rootVisualElement.Q<Label>("ProcessMsg");
        _curNpcData = rootVisualElement.Q<Label>("NpcField");
        _curOtherData = rootVisualElement.Q<Label>("OtherField");
        _resultMessage = rootVisualElement.Q<Label>("ResultField");

        _curNpcData.text = _curOtherData.text = _resultMessage.text = string.Empty;
    }

    #region UpdateMessageFunc

    public static void UpdateProcessMessage(string msg)
    {
        _processResult.text = msg;
    }
    
    public static void UpdateNpcDataMessage(string msg)
    {
        _curNpcData.text = msg;
    }

    public static void UpdateOtherDataMessage(string msg)
    {
        _curOtherData.text = msg;
    }

    public static void UpdateResultMessage(string msg)
    {
        _resultMessage.text = msg;
    }

    #endregion
    private void OnDestroy()
    {
        _isOpen = false;
        GeneratorManager.CloseAllWindow();
    }
}
