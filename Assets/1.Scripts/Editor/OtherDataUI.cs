using System;
using System.IO;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json.Linq;
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
    private Toggle _includeInfo;

    public static string CurOtherData;
    private string _curSelectData;
    private ExcelParser _parser;
    
    private static bool _isOpen;
    private bool _isSearch = false;
    
    public static void CreateWindow()
    {
        _isOpen = true;
        
        var win = GetWindow<OtherDataUI>();
        win.titleContent = new GUIContent("Other Data UI");
        
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

        _includeInfo = rootVisualElement.Q<Toggle>("IncludeInfo");
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
        if (_isSearch == false)
        {
            var col = _nameList.choices.IndexOf(_nameList.value);
            
            _curSelectData = _parser.ConvertValueDataToString(col + 1);
        }
        else
        {
            _curSelectData = _parser.ConvertValueDataToString(_parser.FindColumnWithValue(_nameList.value));
        }
        
        ResultWindow.UpdateOtherDataMessage(_curSelectData);
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
        var result = string.Empty;
        
        if (_includeInfo.value == true)
        {
            var nameMatch = Regex.Match(_curSelectData, @"Name\s*:\s*(.*)");
            var infoMatch = Regex.Match(_curSelectData, @"Info\s*:\s*(.*)");

            var finalText = nameMatch.ToString().Replace("Name : ", $"{GetTitleString()}");
            
            result = $"({finalText} \n {infoMatch}) \n";
            CurOtherData += result;
        }
        else
        {
            var title = GetTitleString();
            result = $"({title} : {_nameList.value}) \n";
            CurOtherData += result;
        }
        
        ResultWindow.UpdateOtherDataMessage(CurOtherData);
        ResultWindow.UpdateProcessMessage($"{_nameList.value} Add Done!!");
    }
    
    private void SearchButtonClickEvent(ClickEvent evt)
    {
        if (_parser.FindColumnWithValue(_searchName.text) <= 0)
        {
            ResultWindow.UpdateProcessMessage($"{_searchName.text} Not Found!!!");
            return;
        }
        
        CurOtherData += _searchName.text + "/";
        
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
        _isSearch = true;
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
        CurOtherData = CurOtherData.TrimEnd('/');
        
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

        var valueList = _parser.GetAllValuesFromKeyOrNull();
        if (valueList == null)
        {
            ResultWindow.UpdateProcessMessage("Key Not Found!!");
            return;
        }
        _nameList.choices = valueList;
        _isSearch = false;
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

    private string GetTitleString()
    {
        var curData = _excelList.value;

        if (curData.Contains("Monster") == true)
        {
            return "MonsterName : ";
        }
        else if (curData.Contains("Item") == true)
        {
            return "ItemName : ";
        }
        else if (curData.Contains("NPC") == true)
        {
            return "TargetNpcName : ";
        }

        return string.Empty;
    }
    
    private void OnDestroy()
    {
        _isOpen = false;
        GeneratorManager.CloseAllWindow();
    }
}
