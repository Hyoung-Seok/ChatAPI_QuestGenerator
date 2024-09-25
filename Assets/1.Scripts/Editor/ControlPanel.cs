using System;
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

    [Header("Slider")] 
    private Slider _temperatureSlider;

    [Header("Button")] 
    private Button _applyBt;
    private Button _sendBt;
    private Button _saveBt;
    private Button _loadBt;
    private Button _createBt;
    private Button _generateBt;

    private string _curExcelData;
    private ExcelParser _parser;
    private QuestGenerator _questGenerator;
    
    private static bool _isOpen;
    
    public static void CreateWindow()
    {
        _isOpen = true;
        
        var win = GetWindow<ControlPanel>();
        win.titleContent = new GUIContent("Control Panel");
        
        win.minSize = new Vector2(550, 510);
        win.maxSize = new Vector2(550, 510);
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

        // slider
        _temperatureSlider = rootVisualElement.Q<Slider>("Temperature");

        // button
        _applyBt = rootVisualElement.Q<Button>("ApplyButton");
        _sendBt = rootVisualElement.Q<Button>("SendButton");
        _saveBt = rootVisualElement.Q<Button>("SaveButton");
        _loadBt = rootVisualElement.Q<Button>("LoadDataButton");
        _createBt = rootVisualElement.Q<Button>("CreateSoButton");
        _generateBt = rootVisualElement.Q<Button>("GenerateLinkageButton");
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
        _generateBt.RegisterCallback<ClickEvent>(GenerateLinkageQuestButton);

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
        ResultCustomWindow.UpdateProcessMessage("GPT 퀘스트 생성 중...");
        
        var message = QuestGeneratorManager.NpcData + "\n targetName : " + QuestGeneratorManager.OtherData;
        ResultCustomWindow.UpdateMessage(message);
        
        QuestGeneratorManager.ResultData = await _questGenerator.CreateJsonMessage(message);
        
        ResultCustomWindow.UpdateMessage(QuestGeneratorManager.ResultData);
        ResultCustomWindow.UpdateProcessMessage("GPT 퀘스트 생성 완료...");
    }

    private void SaveButtonClickEvent(ClickEvent evt)
    {
        if (string.IsNullOrEmpty(QuestGeneratorManager.ResultData) == false)
        {
            _parser.SaveQuestDataInExcel(QuestGeneratorManager.ResultData, NpcDataUI.NpcNotice);
            ResultCustomWindow.UpdateProcessMessage("데이터 저장 완료!");
            return;
        }
        
        ResultCustomWindow.UpdateProcessMessage("데이터 저장 실패!");
    }

    private void LoadExcelDataButtonClickEvent(ClickEvent evt)
    {
        var index = _columnField.value;
        
        if (index <= 1)
        {
            ResultCustomWindow.UpdateProcessMessage("Column Index Error!");
            return;
        }

        _curExcelData = _parser.ConvertValueDataToString(index, true);
        ResultCustomWindow.UpdateMessage(_curExcelData);
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
            ExcelParser.GetValueInJsonString(QuestGeneratorManager.ResultData));
        
        ResultCustomWindow.UpdateProcessMessage("데이터 변환 성공!!");
    }

    private async void GenerateLinkageQuestButton(ClickEvent evt)
    {
        var msg = $"{_curExcelData} \n" + $"연계 퀘스트 생성 : {NpcDataUI.QuestType} \n";
        ResultCustomWindow.UpdateMessage(msg);

        QuestGeneratorManager.ResultData = await _questGenerator.CreateJsonMessage(msg);
        
        ResultCustomWindow.UpdateMessage(QuestGeneratorManager.ResultData);
        ResultCustomWindow.UpdateProcessMessage("연계 퀘스트 생성 완료");
    }
    #endregion

    private void InitParser()
    {
        var path = QuestGeneratorManager.GetFullFilePath(_defaultPath.value, _fileName.value);
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
        QuestGeneratorManager.CloseAllWindow();
    }
}