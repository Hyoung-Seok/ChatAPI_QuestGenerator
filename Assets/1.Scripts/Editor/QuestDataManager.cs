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
    
    private DropdownField _excelFileList;
    private ExcelParser _excelParser;
    private string _curNpcData;
    
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
        
        InitUIFiled();
        
        
        // 각 경로 초기화
        _defaultFilePath = Application.dataPath + "/_ExcelData/";

        // 파일 경로 컨트롤


        // // 엑셀 파일 드랍다운 컨트롤
        // _excelFileList = rootVisualElement.Q<DropdownField>("DataList");
        // GetFileList();

    }

    private void InitUIFiled()
    {
        _excelParser = new ExcelParser();
        
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

    private void NpcDataChangeEvent(ChangeEvent<string> evt)
    {
        var col = _npcNameDropDown.choices.IndexOf(_npcNameDropDown.value) + 1;

        _curNpcData = _excelParser.ConvertValueDataToString(col + 1);
        ResultCustomWindow.UpdateMessage(_curNpcData);
    }

    private void NpcInitValueButtonClickEvent(ClickEvent evt)
    {
        _npcNameDropDown.choices.Clear();
        _excelParser.InitParser(_defaultPathField.value + _npcFileName + ".xlsx");

        var nameList = _excelParser.GetAllBaseValue();
        _npcNameDropDown.choices = nameList;
    }

    private void NpcSearchButtonClickEvent(ClickEvent evt)
    {
        _curNpcData = _excelParser.ConvertValueDataToString(
                _excelParser.FindColumnWitValue(_npcNameSearch.value));
       
        ResultCustomWindow.UpdateMessage(_curNpcData);
    }
    
    private void NpcSaveButtonClickEvent(ClickEvent evt)
    {
        _curNpcData += "Additional Information : " + _npcNoticeTextField.value;
        
        ResultCustomWindow.UpdateMessage(_curNpcData);
        ResultCustomWindow.UpdateProcessMessage("Save Done!!");
    }

    private void GetFileList()
    {
        if (Directory.Exists(_defaultFilePath))
        {
            var files = Directory.GetFiles(_defaultFilePath, "*.xlsx");
            
            if(files.Length > 0)
            {
                _excelFileList.choices.Clear();

                foreach (var filePath in files)
                {
                    var name = Path.GetFileName(filePath);
                    _excelFileList.choices.Add(name);
                }
            }
        }
    }
}
