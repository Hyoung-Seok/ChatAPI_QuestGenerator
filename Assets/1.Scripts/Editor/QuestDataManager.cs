using System;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestDataManager : EditorWindow
{
    private const int KEY_ROW = 0;
    
    [Header("File Path")]
    private string _defaultFilePath;
    private string _npcFileName = "NPCData";

    [Header("UI Field")] 
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    
    // 파일 경로 입력 필드
    private TextField _defaultPathField;
    private TextField _npcDataFileName;
    
    // NPC UI
    private DropdownField _npcNameDropDown;
    private TextField _npcNameSearch;
    private EnumField _questType;
    private TextField _npcNoticeTextField;
    private Button _npcInitValueBt;
    private Button _npcSearchBt;
    private Button _npcSaveBt;
    
    private ExcelParser _npcExcelParser;
    private ExcelParser _etcExcelParser;
    private string _curNpcData;
    
    // Other UI
    private DropdownField _excelDropDown;
    private DropdownField _etcNameDropDown;
    private TextField _etcSearchField;
    private MinMaxSlider _minMaxSlider;
    private Label _minMaxInfo;
    private TextField _etcNotice;
    private Button _etcValueInitBt;
    private Button _addBt;
    private Button _etcSearchBt;
    private Button _applyBt;
    private Button _resetButton;
    private Button _etcSaveButton;
    private string _curEtcData;
    
    
    [MenuItem("OpenAI/QuestData")]
    public static void ShowEditor()
    {
        var window = GetWindow<QuestDataManager>();
        window.titleContent = new GUIContent("QuestDataManager");
        
        ResultCustomWindow.ShowResultWindow();

        // window.minSize = new Vector2(800, 1000);
        // window.maxSize = new Vector2(800, 1000);
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        InitNpcUIField();
        InitOtherUIField();

        // // 엑셀 파일 드랍다운 컨트롤
        // _excelFileList = rootVisualElement.Q<DropdownField>("DataList");
        // GetFileList();

    }

    private void InitNpcUIField()
    {
        _npcExcelParser = new ExcelParser();
        
        //경로 초기화
        _defaultFilePath = Application.dataPath + "/_ExcelData/";
        
        _defaultPathField = rootVisualElement.Q<TextField>("DefaultPath");
        _defaultPathField.value = _defaultFilePath;
        
        _npcDataFileName = rootVisualElement.Q<TextField>("NpcDataFileName");
        _npcDataFileName.value = _npcFileName;
        
        // 인풋 필드 초기화
        _npcNameSearch = rootVisualElement.Q<TextField>("NpcSearch");
        _npcNoticeTextField = rootVisualElement.Q<TextField>("NpcNotice");
        
        // 버튼 초기화
        _npcInitValueBt = rootVisualElement.Q<Button>("NpcInitButton");
        _npcInitValueBt.RegisterCallback<ClickEvent>(NpcInitValueButtonClickEvent);

        _npcSearchBt = rootVisualElement.Q<Button>("NpcSearchButton");
        _npcSearchBt.RegisterCallback<ClickEvent>(NpcSearchButtonClickEvent);

        _npcSaveBt = rootVisualElement.Q<Button>("NpcSaveButton");
        _npcSaveBt.RegisterCallback<ClickEvent>(NpcSaveButtonClickEvent);
        
        // 드랍다운 초기화
        _npcNameDropDown = rootVisualElement.Q<DropdownField>("NpcNameField");
        _npcNameDropDown.RegisterValueChangedCallback(NpcDataChangeEvent);
        _questType = rootVisualElement.Q<EnumField>("QuestType");
        _questType.Init(EQuestType.Fight);
    }

    private void InitOtherUIField()
    {
        // 드랍다운
        _excelDropDown = rootVisualElement.Q<DropdownField>("ExcelList");
        GetFileList();
        _etcNameDropDown = rootVisualElement.Q<DropdownField>("EctNameField");
        
        // 버튼
        _etcValueInitBt = rootVisualElement.Q<Button>("InitButton");
        _etcValueInitBt.RegisterCallback<ClickEvent>(OnEctInitValueButtonClickEvent);

        _addBt = rootVisualElement.Q<Button>("AddButton");
        _addBt.RegisterCallback<ClickEvent>(OnAddButtonClickEvent);

        _etcSearchBt = rootVisualElement.Q<Button>("SearchButton");
        _etcSearchBt.RegisterCallback<ClickEvent>(OnSearchButtonClickEvent);

        _resetButton = rootVisualElement.Q<Button>("ResetButton");
        _resetButton.RegisterCallback<ClickEvent>(OnResetButtonClickEvent);

        _applyBt = rootVisualElement.Q<Button>("ApplyButton");
        _applyBt.RegisterCallback<ClickEvent>(OnApplyButtonClickEvent);

        _etcSaveButton = rootVisualElement.Q<Button>("SaveEtcButton");
        _etcSaveButton.RegisterCallback<ClickEvent>(OnEtcSaveButtonClickEvent);
        
        // 입력 필드
        _etcSearchField = rootVisualElement.Q<TextField>("NameSearch");
        _etcNotice = rootVisualElement.Q<TextField>("OtherNotice");
        
        // 기타
        _minMaxSlider = rootVisualElement.Q<MinMaxSlider>("LevelSettingSlider");
        _minMaxSlider.RegisterValueChangedCallback(OnMinMaxSliderValueChangeEvent);

        _minMaxInfo = rootVisualElement.Q<Label>("MinMaxInfo");

    }

    #region NPC_DATA_UI
    private void NpcDataChangeEvent(ChangeEvent<string> evt)
    {
        var col = _npcNameDropDown.choices.IndexOf(_npcNameDropDown.value) + 1;

        _curNpcData = _npcExcelParser.ConvertValueDataToString(col + 1);
        ResultCustomWindow.UpdateMessage(_curNpcData);
    }

    private void NpcInitValueButtonClickEvent(ClickEvent evt)
    {
        _npcNameDropDown.choices.Clear();
        _npcExcelParser.InitParser(_defaultPathField.value + _npcFileName + ".xlsx");

        var nameList = _npcExcelParser.GetAllBaseValue();
        _npcNameDropDown.choices = nameList;
    }

    private void NpcSearchButtonClickEvent(ClickEvent evt)
    {
        _curNpcData = _npcExcelParser.ConvertValueDataToString(
                _npcExcelParser.FindColumnWitValue(_npcNameSearch.value));
       
        ResultCustomWindow.UpdateMessage(_curNpcData);
    }
    
    private void NpcSaveButtonClickEvent(ClickEvent evt)
    {
        _curNpcData += "Additional Information : " + _npcNoticeTextField.value;
        
        ResultCustomWindow.UpdateMessage(_curNpcData);
        ResultCustomWindow.UpdateProcessMessage("Save Done!!");
    }
    #endregion

    #region OTHER_DATA_UI

    private void OnEctInitValueButtonClickEvent(ClickEvent evt)
    {
        _etcExcelParser = new ExcelParser();
        _etcExcelParser.InitParser(_defaultFilePath + _excelDropDown.value);

        _etcNameDropDown.choices.Clear();
        _etcNameDropDown.choices = _etcExcelParser.GetAllBaseValue();
    }

    private void OnAddButtonClickEvent(ClickEvent evt)
    {
        _curEtcData += _etcNameDropDown.value + " / ";
        
        ResultCustomWindow.UpdateMessage(_curEtcData);
        ResultCustomWindow.UpdateProcessMessage($"{_etcNameDropDown.value} Add Done!!");
    }

    private void OnSearchButtonClickEvent(ClickEvent evt)
    {
        if (_etcExcelParser.FindColumnWitValue(_etcSearchField.text) <= 0)
        {
            ResultCustomWindow.UpdateProcessMessage($"{_etcSearchField.text} Not Found!!!");
            return;
        }
        
        _curEtcData += _etcSearchField.text + " / ";
        
        ResultCustomWindow.UpdateMessage(_curEtcData);
        ResultCustomWindow.UpdateProcessMessage($"{_etcSearchField.text} Add Done!!");
    }

    private void OnResetButtonClickEvent(ClickEvent evt)
    {
        _curEtcData = string.Empty;
        
        ResultCustomWindow.UpdateMessage(_curEtcData);
        ResultCustomWindow.UpdateProcessMessage($"Reset Done!!");
    }

    private void OnMinMaxSliderValueChangeEvent(ChangeEvent<Vector2> evt)
    {
        var min = _minMaxSlider.minValue;
        var max = _minMaxSlider.maxValue;

        _minMaxInfo.text = $"Min : {(int)min}      Max : {(int)max}";
    }

    private void OnApplyButtonClickEvent(ClickEvent evt)
    {
        var result =
            _etcExcelParser.GetValuesByLevel((int)_minMaxSlider.minValue,
                (int)_minMaxSlider.maxValue);

        if (result == null)
        {
            ResultCustomWindow.UpdateProcessMessage($"Value Not Found!!");
            return;
        }
        
        _etcNameDropDown.choices.Clear();
        _etcNameDropDown.choices = result;
    }

    private void OnEtcSaveButtonClickEvent(ClickEvent evt)
    {
        if (string.IsNullOrEmpty(_etcNotice.value) == false)
        {
            _curEtcData += '\n' + "Additional Information : " + _etcNotice.value;
        }
        
        ResultCustomWindow.UpdateMessage(_curEtcData);
        ResultCustomWindow.UpdateProcessMessage("Save Done!!");
    }
    
    #endregion

    private void GetFileList()
    {
        if (Directory.Exists(_defaultFilePath) == false)
        {
            return;
        }
        
        var files = Directory.GetFiles(_defaultFilePath, "*.xlsx");
            
        if(files.Length <= 0)
        {
            return;
        }
        
        _excelDropDown.choices.Clear();

        foreach (var filePath in files)
        {
            var name = Path.GetFileName(filePath);
            _excelDropDown.choices.Add(name);
        }
    }

    private void OnDestroy()
    {
        ResultCustomWindow.CloseWindow();
    }
}
