using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class OtherDataUI : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset = default;

    [Header("Input Field")] 
    private TextField _otherNotice;
    private TextField _searchName;
    private Label _levelInfo;

    [Header("Dropdown")] 
    private DropdownField _excelList;
    private DropdownField _nameList;

    [Header("Slider")] 
    private MinMaxSlider _levelSlider;

    [Header("Button")] 
    private Button _initBt;
    private Button _addBt;
    private Button _searchBt;
    private Button _applyBt;
    private Button _resetBt;
    private Button _saveBt;

    public static string CurOtherData;
    private ExcelParser _parser;
    
    private static bool _isOpen;
    
    public static void CreateWindow()
    {
        _isOpen = true;
        
        var win = GetWindow<OtherDataUI>();
        win.titleContent = new GUIContent("Other Data UI");
        
        win.minSize = new Vector2(550, 510);
        win.maxSize = new Vector2(550, 510);
    }

    public static void ResetCurrentData()
    {
        CurOtherData = string.Empty;
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        InitUI();
        InitEventAndValue();
    }

    private void InitUI()
    {
        // text field
        _searchName = rootVisualElement.Q<TextField>("SearchName");
        _levelInfo = rootVisualElement.Q<Label>("CurrentLevelInfo");
        _otherNotice = rootVisualElement.Q<TextField>("OtherNotice");
        
        // dropdown
        _excelList = rootVisualElement.Q<DropdownField>("ExcelListDropdown");
        _nameList = rootVisualElement.Q<DropdownField>("NameListDropdown");
        
        // slider
        _levelSlider = rootVisualElement.Q<MinMaxSlider>("LeverSlider");
        
        // button
        _initBt = rootVisualElement.Q<Button>("InitValueButton");
        _addBt = rootVisualElement.Q<Button>("AddButton");
        _searchBt = rootVisualElement.Q<Button>("SearchButton");
        _applyBt = rootVisualElement.Q<Button>("ApplyButton");
        _resetBt = rootVisualElement.Q<Button>("ResetButton");
        _saveBt = rootVisualElement.Q<Button>("SaveButton");
    }

    private void InitEventAndValue()
    {
        GetFileList();
        
        // value change event
        _nameList.RegisterValueChangedCallback(NameListValueChangeEvent);
        _levelSlider.RegisterValueChangedCallback(LevelSliderValueChangeEvent);
        
        // click event
        _initBt.RegisterCallback<ClickEvent>(InitButtonClickEvent);
        _addBt.RegisterCallback<ClickEvent>(AddButtonClickEvent);
        _searchBt.RegisterCallback<ClickEvent>(SearchButtonClickEvent);
        _applyBt.RegisterCallback<ClickEvent>(ApplyButtonClickEvent);
        _resetBt.RegisterCallback<ClickEvent>(ResetButtonClickEvent);
        _saveBt.RegisterCallback<ClickEvent>(SaveButtonClickEvent);
    }

    #region Event

    private void NameListValueChangeEvent(ChangeEvent<string> evt)
    {
        var col = _nameList.choices.IndexOf(_nameList.value) + 1;
        var data = _parser.ConvertValueDataToString(col + 1);
        
        ResultWindow.UpdateOtherDataMessage(data);
    }

    private void LevelSliderValueChangeEvent(ChangeEvent<Vector2> evt)
    {
        var value = _levelSlider.value;
        _levelInfo.text = $"Min : {(int)value.x} \t Max : {(int)value.y}";
    }
    
    private void InitButtonClickEvent(ClickEvent evt)
    {
        InitParser();
    }

    private void AddButtonClickEvent(ClickEvent evt)
    {
        CurOtherData += _nameList.value + " / ";
        
        ResultWindow.UpdateOtherDataMessage(CurOtherData);
        ResultWindow.UpdateProcessMessage($"{_nameList.value} Add Done!!");
    }
    
    private void SearchButtonClickEvent(ClickEvent evt)
    {
        if (_parser.FindColumnWitValue(_searchName.text) <= 0)
        {
            ResultWindow.UpdateProcessMessage($"{_searchName.text} Not Found!!!");
            return;
        }
        
        CurOtherData += _searchName.text + " / ";
        
        ResultWindow.UpdateOtherDataMessage(CurOtherData);
        ResultWindow.UpdateProcessMessage($"{_searchName.text} Add Done!!");
    }

    private void ApplyButtonClickEvent(ClickEvent evt)
    {
        var level = _levelSlider.value;
        var list = _parser.GetValuesByLevel((int)level.x, (int)level.y);

        if (list == null)
        {
            ResultWindow.UpdateProcessMessage($"Value Not Found!!");
            return;
        }
        
        _nameList.choices.Clear();
        _nameList.choices = list;
    }

    private void ResetButtonClickEvent(ClickEvent evt)
    {
        CurOtherData = string.Empty;
        GeneratorManager.OtherData = string.Empty;
        
        ResultWindow.UpdateOtherDataMessage(CurOtherData);
        ResultWindow.UpdateProcessMessage($"Reset Done!!");
    }

    private void SaveButtonClickEvent(ClickEvent evt)
    {
        if (string.IsNullOrEmpty(_otherNotice.value) == false)
        {
            CurOtherData += '\n' + "Additional Information : " + _otherNotice.value;
        }

        GeneratorManager.OtherData = CurOtherData;
        ResultWindow.UpdateOtherDataMessage(GeneratorManager.OtherData);
        ResultWindow.UpdateProcessMessage("Save Done!!");
    }

    #endregion

    private void InitParser()
    {
        var path = GeneratorManager.GetFullFilePath("_ExcelData", _excelList.value);
        _parser = new ExcelParser(path);
        
        _nameList.choices.Clear();
        _nameList.choices = _parser.GetAllValuesFromKey();
    }

    private void GetFileList()
    {
        var path = Path.Combine(Application.dataPath, "_ExcelData");
        
        if (Directory.Exists(path) == false) return;
        
        var files = Directory.GetFiles(path, "*.xlsx");
            
        if(files.Length <= 0)
        {
            return;
        }
        
        _excelList.choices.Clear();

        foreach (var filePath in files)
        {
            var fileName = Path.GetFileName(filePath);
            _excelList.choices.Add(fileName);
        }
    }
    
    public static void CloseWindow()
    {
        if(_isOpen == false) return;
        
        var window = GetWindow<OtherDataUI>();
        window.Close();
    }
    
    private void OnDestroy()
    {
        _isOpen = false;
        GeneratorManager.CloseAllWindow();
    }
}
