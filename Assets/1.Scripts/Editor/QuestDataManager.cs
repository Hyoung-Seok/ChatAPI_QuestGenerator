using System;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestDataManager : EditorWindow
{
    [Header("File Path")]
    private string _defaultFilePath;

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
    
    // Generate Data UI
    private TextField _questFileName;
    private Button _sendMsgButton;
    private Button _saveToExcelButton;
    private Button _createSoButton;
    
    // GPT Setting UI
    private Slider _tempSlider;
    private Label _tempInfo;
    private IntegerField _colInputField;
            
    private ExcelParser _npcExcelParser;
    private ExcelParser _etcExcelParser;
    private ExcelParser _questExcelParser;
    
    private QuestGenerator _questGenerator;
    private string _curNpcData;
    private string _resultData;
    
    [MenuItem("OpenAI/QuestData")]
    public static void ShowEditor()
    {
        var window = GetWindow<QuestDataManager>();
        window.titleContent = new GUIContent("QuestDataManager");
        
        ResultCustomWindow.ShowResultWindow();

         window.minSize = new Vector2(670, 1200);
         window.maxSize = new Vector2(670, 1200);
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);

        _defaultFilePath = Application.dataPath;
        
        InitNpcUIField();
        InitOtherUIField();
        InitGptSettingField();
        InitGenerateQuestUIField();
    }

    private void InitNpcUIField()
    {
        _npcExcelParser = new ExcelParser();
        
        //경로 초기화
        _defaultPathField = rootVisualElement.Q<TextField>("DefaultPath");
        _npcDataFileName = rootVisualElement.Q<TextField>("NpcDataFileName");
        
        _defaultFilePath = Path.Combine(_defaultFilePath, _defaultPathField.value);
        
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

    private void InitGptSettingField()
    {
        _tempSlider = rootVisualElement.Q<Slider>("TempSlider");
        _tempSlider.RegisterValueChangedCallback(OnTempSliderChangeEvent);

        _tempInfo = rootVisualElement.Q<Label>("TempInfo");

        _questGenerator = new QuestGenerator(EChatModel.GPT4_TERBO, 0);
    }

    private void InitGenerateQuestUIField()
    {
        _questFileName = rootVisualElement.Q<TextField>("QuestFileName");
        
        // 경로 및 파서 초기화
        _questExcelParser = new ExcelParser();

        var path = Path.Combine(_defaultFilePath, _questFileName.value) + ".xlsx";
        _questExcelParser.InitParser(path);

        _sendMsgButton = rootVisualElement.Q<Button>("SendMessageButton");
        _sendMsgButton.RegisterCallback<ClickEvent>(OnSendButtonClickEvent);

        _saveToExcelButton = rootVisualElement.Q<Button>("SaveToExcelButton");
        _saveToExcelButton.RegisterCallback<ClickEvent>(OnSaveToExcelButtonClickEvent);

        _createSoButton = rootVisualElement.Q<Button>("CreateSOButton");
        _createSoButton.RegisterCallback<ClickEvent>(OnCreateSoObjectClickButton);

        _colInputField = rootVisualElement.Q<IntegerField>("ColInputField");
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

        var path = Path.Combine(_defaultFilePath, _npcDataFileName.value);
        _npcExcelParser.InitParser(path + ".xlsx");

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
        var path = Path.Combine(_defaultFilePath, _excelDropDown.value);
        _etcExcelParser.InitParser(path);

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

    #region SETTING_GPT_UI

    private void OnTempSliderChangeEvent(ChangeEvent<float> evt)
    {
        _tempInfo.text = $"Current Temperature : {_tempSlider.value}";
        _questGenerator.ChangeTemperature(_tempSlider.value);
    }
    
    #endregion

    #region GENERATE_DATA_UI

    private async void OnSendButtonClickEvent(ClickEvent evt)
    {
        ResultCustomWindow.UpdateProcessMessage("GPT 퀘스트 생성 중...");
        
        var message = _curNpcData + $"\n Type : {_questType.text} \n" + _curEtcData;
        _resultData = await _questGenerator.CreateJsonMessage(message);
        
        ResultCustomWindow.UpdateMessage(_resultData);
        ResultCustomWindow.UpdateProcessMessage("GPT 퀘스트 생성 완료...");
    }

    private void OnSaveToExcelButtonClickEvent(ClickEvent evt)
    {
        if (string.IsNullOrEmpty(_resultData) == false)
        {
            _questExcelParser.SaveQuestDataInExcel(_resultData, _npcNoticeTextField.value);
            ResultCustomWindow.UpdateProcessMessage("데이터 저장 완료!");
            return;
        }
        
        ResultCustomWindow.UpdateProcessMessage("데이터 저장 실패!");
    }

    private void OnCreateSoObjectClickButton(ClickEvent evt)
    {
        if (_colInputField.value > 1)
        {
            var result= _questExcelParser.ConvertValueDataToList(_colInputField.value);

            foreach (var value in result)
            {
                Debug.Log(value);
            }
            
            QuestScriptableGenerator.CreateAndSaveScriptableObj<QuestData>(
                _questExcelParser.ConvertValueDataToList(_colInputField.value));

            return;
        }
        
        QuestScriptableGenerator.CreateAndSaveScriptableObj<QuestData>(
            ExcelParser.GetValueInJsonString(_resultData));
        
        ResultCustomWindow.UpdateProcessMessage("데이터 변환 성공!!");
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
            var fileName = Path.GetFileName(filePath);
            _excelDropDown.choices.Add(fileName);
        }
    }

    private void OnDestroy()
    {
        ResultCustomWindow.CloseWindow();
    }
}
