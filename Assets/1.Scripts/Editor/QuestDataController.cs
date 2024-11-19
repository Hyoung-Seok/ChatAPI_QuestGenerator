using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

public class QuestDataController : EditorWindow
{
    [Header("Content")] 
    private DropdownField _npcNameDropdown;
    private DropdownField _questDropdown;
    private Button _questStateChangeToClear;
    private Button _resetQuestData;
    private Button _resetAllQuestData;
    
    [SerializeField] private VisualTreeAsset visualTreeAsset = default;
    private static Dictionary<string, Dictionary<string, QuestData>> _questData;

    [MenuItem("OpenAI/QuestDataController")]
    public static void CreateWindow()
    {
        var win = GetWindow<QuestDataController>();
        win.titleContent = new GUIContent("QuestData Controller");
        
        win.minSize = new Vector2(500, 450);
        win.maxSize = new Vector2(500, 450);
    }

    public void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        InitData();
        InitButton();
    }

    private void InitData()
    {
        _questData = new Dictionary<string, Dictionary<string, QuestData>>();

        var path = "Assets/_QuestData";
        var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { path });
        
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var questData = AssetDatabase.LoadAssetAtPath<QuestData>(assetPath);

            if(questData == null) continue;

            if (_questData.ContainsKey(questData.NpcName) == false)
            {
                var dic = new Dictionary<string, QuestData> { { questData.Title, questData } };
                _questData.Add(questData.NpcName, dic);
            }
            else
            {
                _questData[questData.NpcName].Add(questData.Title, questData);
            }
        }

        _npcNameDropdown = rootVisualElement.Q<DropdownField>("QuestNpcNameList");
        _questDropdown = rootVisualElement.Q<DropdownField>("QuestDataList");

        _npcNameDropdown.choices = _questData.Keys.ToList();
        _npcNameDropdown.RegisterValueChangedCallback(OnValueChange);
    }

    private void InitButton()
    {
        _questStateChangeToClear = rootVisualElement.Q<Button>("QuestClearButton");
        _resetQuestData = rootVisualElement.Q<Button>("ResetQuestDataButton");
        _resetAllQuestData = rootVisualElement.Q<Button>("ResetAllQuestDataButton");
        
        _questStateChangeToClear.RegisterCallback<ClickEvent>(ChangeQuestStateToClear);
        _resetQuestData.RegisterCallback<ClickEvent>(ResetData);
        _resetAllQuestData.RegisterCallback<ClickEvent>(ResetAllData);
    }

    private void OnValueChange(ChangeEvent<string> evt)
    {
        _questDropdown.choices.Clear();
        _questDropdown.choices = _questData[_npcNameDropdown.value].Keys.ToList();
    }

    private void ChangeQuestStateToClear(ClickEvent evt)
    {
        var npcName = _npcNameDropdown.value;

        if (_questData[npcName].TryGetValue(_questDropdown.value, out var data) == false)
        {
            return;
        }

        data.CurQuestState = EQuestState.Completion;

        if (data.ChainQuest.Key != default)
        {
            data.ChainQuest.Value.CurQuestState = EQuestState.Completion;
        }
    }

    private void ResetData(ClickEvent evt)
    {
        var npcName = _npcNameDropdown.value;

        if (_questData[npcName].TryGetValue(_questDropdown.value, out var data) == false)
        {
            return;
        }

        foreach (var targetInfo in data.TargetInfos)
        {
            targetInfo.CurTargetCount = 0;
        }

        data.CurQuestState = EQuestState.Start;
    }

    private void ResetAllData(ClickEvent evt)
    {
        foreach (var dic in _questData)
        {
            foreach (var questData in dic.Value)
            {
                foreach (var target in questData.Value.TargetInfos)
                {
                    target.CurTargetCount = 0;
                }

                questData.Value.CurQuestState = EQuestState.Start;
            }
        }
    }
}
