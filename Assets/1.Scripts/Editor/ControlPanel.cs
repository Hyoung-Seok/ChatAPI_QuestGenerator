using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlPanel : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset = default;

    [Header("TextField")] 
    private TextField _defaultPath;
    private TextField _fileName;
    private IntegerField _columnField;
    private Label _infoLabel;
    private TextField _linkageNotice;

    [Header("Toggle")] 
    private Toggle _ignoreData;
    
    [Header("Slider")] 
    private Slider _temperatureSlider;

    [Header("Button")] 
    private Button _applyBt;
    private Button _sendBt;
    private Button _saveBt;
    private Button _loadBt;
    private Button _createBt;
    private Button _generateCurrentBt;
    private Button _generateExcelBt;
    
    private ExcelParser _parser;
    private QuestGenerator _questGenerator;
    private string _curExcelData;
    
    private static bool _isOpen;
    
    public static void CreateWindow()
    {
        _isOpen = true;
        
        var win = GetWindow<ControlPanel>();
        win.titleContent = new GUIContent("Control Panel");
        
        win.minSize = new Vector2(550, 800);
        win.maxSize = new Vector2(550, 800);
    }

    private void CreateGUI()
    {
        visualTreeAsset.CloneTree(rootVisualElement);
        
        InitUI();
        InitEventAndField();
    }

    private void InitUI()
    {
        // field
        _defaultPath = rootVisualElement.Q<TextField>("DefaultPath");
        _fileName = rootVisualElement.Q<TextField>("FileName");
        _columnField = rootVisualElement.Q<IntegerField>("ExcelColumn");
        _infoLabel = rootVisualElement.Q<Label>("CurrentInfo");
        _linkageNotice = rootVisualElement.Q<TextField>("LinkageNotice");
        
        // toggle
        _ignoreData = rootVisualElement.Q<Toggle>("IgnoreToggle");

        // slider
        _temperatureSlider = rootVisualElement.Q<Slider>("Temperature");

        // button
        _applyBt = rootVisualElement.Q<Button>("ApplyButton");
        _sendBt = rootVisualElement.Q<Button>("SendButton");
        _saveBt = rootVisualElement.Q<Button>("SaveButton");
        _loadBt = rootVisualElement.Q<Button>("LoadDataButton");
        _createBt = rootVisualElement.Q<Button>("CreateSoButton");

        _generateCurrentBt = rootVisualElement.Q<Button>("LinkageFromCurrentButton");
        _generateExcelBt = rootVisualElement.Q<Button>("LinkageFromExcelButton");
    }

    private void InitEventAndField()
    {
        InitParser();
        _questGenerator = new QuestGenerator(EChatModel.GPT4_TERBO, 0);
        
        _applyBt.RegisterCallback<ClickEvent>(ApplyButtonClickEvent);
        _sendBt.RegisterCallback<ClickEvent>(SendButtonClickEvent);
        _saveBt.RegisterCallback<ClickEvent>(SaveButtonClickEvent);
        _loadBt.RegisterCallback<ClickEvent>(LoadExcelDataButtonClickEvent);
        _createBt.RegisterCallback<ClickEvent>(CreateSoButtonClickEvent);
        _generateCurrentBt.RegisterCallback<ClickEvent>(GenerateFromCurrentLinkageQuestButton);
        _generateExcelBt.RegisterCallback<ClickEvent>(GenerateFromExcelLinkageQuestButton);

        _temperatureSlider.RegisterValueChangedCallback(TemperatureValueChangeEvent);
    }

    #region

    private void TemperatureValueChangeEvent(ChangeEvent<float> evt)
    {
        _infoLabel.text = $"Current Temperature : {_temperatureSlider.value}";
        _questGenerator.ChangeTemperature(_temperatureSlider.value);
    }

    private void ApplyButtonClickEvent(ClickEvent evt)
    {
        InitParser();
    }

    private async void SendButtonClickEvent(ClickEvent evt)
    {
        ResultWindow.UpdateProcessMessage("GPT 퀘스트 생성 중...");
        
        var message = GeneratorManager.NpcData + "\n targetName : " + GeneratorManager.OtherData;
        ResultWindow.UpdateMessage(message);
        
        GeneratorManager.ResultData = await _questGenerator.CreateJsonMessage(message);
        OtherDataUI.CurOtherData = string.Empty;
        GeneratorManager.OtherData = string.Empty;
        
        ResultWindow.UpdateMessage(GeneratorManager.ResultData);
        ResultWindow.UpdateProcessMessage("GPT 퀘스트 생성 완료...");
    }

    private void SaveButtonClickEvent(ClickEvent evt)
    {
        if (string.IsNullOrEmpty(GeneratorManager.ResultData) == false)
        {
            _parser.SaveQuestDataInExcel(GeneratorManager.ResultData, NpcDataUI.NpcNotice);
            ResultWindow.UpdateProcessMessage("데이터 저장 완료!");
            return;
        }
        
        ResultWindow.UpdateProcessMessage("데이터 저장 실패!");
    }

    private void LoadExcelDataButtonClickEvent(ClickEvent evt)
    {
        var index = _columnField.value;
        
        if (index <= 1)
        {
            ResultWindow.UpdateProcessMessage("Column Index Error!");
            return;
        }

        _curExcelData = _parser.ConvertValueDataToString(index, _ignoreData.value);
        ResultWindow.UpdateMessage(_curExcelData);
    }

    private void CreateSoButtonClickEvent(ClickEvent evt)
    {
        if (_columnField.value > 1)
        {
            QuestScriptableGenerator.CreateAndSaveScriptableObj<QuestData>(
                _parser.ConvertValueDataToList(_columnField.value));

            return;
        }
        
        QuestScriptableGenerator.CreateAndSaveScriptableObj<QuestData>(
            ExcelParser.GetValueInJsonString(GeneratorManager.ResultData));
        
        ResultWindow.UpdateProcessMessage("데이터 변환 성공!!");
    }

    private async void GenerateFromCurrentLinkageQuestButton(ClickEvent evt)
    {
        var msg = new StringBuilder(GeneratorManager.ResultData + '\n');
        msg.AppendLine($"연계 퀘스트 생성 : {NpcDataUI.QuestType}");

        if (string.IsNullOrEmpty(GeneratorManager.OtherData) == false)
        {
            msg.AppendLine("Target Name : " + GeneratorManager.OtherData);
        }

        if (string.IsNullOrEmpty(_linkageNotice.value) == false)
        {
            msg.AppendLine("Additional Information : " + _linkageNotice.value);
        }
        
        ResultWindow.UpdateBothMessage(msg.ToString(), "연계 퀘스트 생성중..");
        GeneratorManager.ResultData = await _questGenerator.CreateJsonMessage(msg.ToString());
        
        ResultWindow.UpdateBothMessage(GeneratorManager.ResultData, "연계 퀘스트 생성 완료!!");
    }

    private async void GenerateFromExcelLinkageQuestButton(ClickEvent evt)
    {
        if (string.IsNullOrEmpty(_curExcelData) == true)
        {
            ResultWindow.UpdateProcessMessage("Excel Data Is Empty!");
            return;
        }

        var msg = new StringBuilder(_curExcelData);
        msg.AppendLine($"연계 퀘스트 생성 : {NpcDataUI.QuestType}");

        if (string.IsNullOrEmpty(GeneratorManager.OtherData) == false)
        {
            msg.AppendLine("Target Name : " + GeneratorManager.OtherData);
        }

        if (string.IsNullOrEmpty(_linkageNotice.value) == false)
        {
            msg.AppendLine("Additional Information : " + _linkageNotice.value);
        }
        
        ResultWindow.UpdateBothMessage(msg.ToString(), "연계 퀘스트 생성중..");
        GeneratorManager.ResultData = await _questGenerator.CreateJsonMessage(msg.ToString());
        
        ResultWindow.UpdateBothMessage(GeneratorManager.ResultData, "연계 퀘스트 생성 완료!!");
    }
    #endregion

    private void InitParser()
    {
        var path = GeneratorManager.GetFullFilePath(_defaultPath.value, _fileName.value);
        _parser = new ExcelParser(path);
    }
    
    public static void CloseWindow()
    {
        if(_isOpen == false) return;
        
        var window = GetWindow<ControlPanel>();
        window.Close();
    }

    private void OnDestroy()
    {
        _isOpen = false;
        GeneratorManager.CloseAllWindow();
    }
}
