using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcDataUI : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    
    [Header("Input Field")]
    private TextField _dataPath;
    private TextField _fileName;
    private TextField _searchName;
    private TextField _npcNotice;

    [Header("Dropdown")] 
    private DropdownField _npcNameList;
    private EnumField _questType;

    [Header("Button")] 
    private Button _initBt;
    private Button _searchBt;
    private Button _resetBt;
    private Button _saveBt;

    private string _fullPath;
    private string _curNpcData;
    private ExcelParser _parser;
    
    public static void CreateWindow()
    {
        var win = GetWindow<NpcDataUI>();
        win.titleContent = new GUIContent("Npc Data UI");

        win.minSize = new Vector2(550, 510);
        win.maxSize = new Vector2(550, 510);
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        InitUI();
        InitEventAndValue();
    }

    private void InitUI()
    {
        // TextField
        _dataPath = rootVisualElement.Q<TextField>("NpcFilePath");
        _fileName = rootVisualElement.Q<TextField>("NpcFileName");
        _searchName = rootVisualElement.Q<TextField>("NpcNameSearch");
        _npcNotice = rootVisualElement.Q<TextField>("NpcNotice");
        
        // dropdown
        _npcNameList = rootVisualElement.Q<DropdownField>("NpcNameDropdown");
        _questType = rootVisualElement.Q<EnumField>("QuestType");
        _questType.Init(EQuestType.Fight);
        
        // button
        _initBt = rootVisualElement.Q<Button>("InitButton");
        _searchBt = rootVisualElement.Q<Button>("SearchButton");
        _resetBt = rootVisualElement.Q<Button>("ResetButton");
        _saveBt = rootVisualElement.Q<Button>("SaveButton");
    }

    private void InitEventAndValue()
    {
        InitParser();
        _npcNameList.choices = _parser.GetAllValuesFromKey();

        _npcNameList.RegisterValueChangedCallback(NpcNameDropdownValueChangeEvent);
        _initBt.RegisterCallback<ClickEvent>(InitValueClickEvent);
        _searchBt.RegisterCallback<ClickEvent>(SearchButtonClickEvent);
        _resetBt.RegisterCallback<ClickEvent>(ResetButtonClickEvent);
        _saveBt.RegisterCallback<ClickEvent>(SaveButtonClickEvent);
    }

    #region Event

    private void NpcNameDropdownValueChangeEvent(ChangeEvent<string> evt)
    {
        if(_parser == null) return;
        
        var col = _npcNameList.choices.IndexOf(_npcNameList.value) + 1;
        _curNpcData = _parser.ConvertValueDataToString(col + 1);
        
        ResultCustomWindow.UpdateMessage(_curNpcData);
    }

    private void InitValueClickEvent(ClickEvent evt)
    {
        InitParser();
        
        _npcNameList.choices.Clear();
        _npcNameList.choices = _parser.GetAllValuesFromKey();
    }

    private void SearchButtonClickEvent(ClickEvent evt)
    {
        _curNpcData = _parser.ConvertValueDataToString(_parser.FindColumnWitValue(_searchName.value));
        ResultCustomWindow.UpdateMessage(_curNpcData);
    }

    private void ResetButtonClickEvent(ClickEvent evt)
    {
        QuestGeneratorManager.NpcData = string.Empty;
        
        ResultCustomWindow.UpdateMessage(string.Empty);
        ResultCustomWindow.UpdateProcessMessage("Npc Data Reset Done!!");
    }

    private void SaveButtonClickEvent(ClickEvent evt)
    {
        QuestGeneratorManager.NpcData = _curNpcData;
        
        if (string.IsNullOrEmpty(_npcNotice.value) == false)
        {
            QuestGeneratorManager.NpcData += 
                "Additional Information : " + _npcNotice.value;
        }
        
        ResultCustomWindow.UpdateMessage(QuestGeneratorManager.NpcData);
        ResultCustomWindow.UpdateProcessMessage("Save Done!!");
    }

    #endregion
    
    private void InitParser()
    {
        _fullPath = QuestGeneratorManager.GetFullFilePath(_dataPath.value, _fileName.value);
        _parser = new ExcelParser(_fullPath);
    }
}
