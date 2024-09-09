using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// OpenAPI 파일 등록 클래스
/// file을 저장, 삭제, 변환하는 역할.
/// </summary>
public class API_FileController : OpenAIController
{
    [Header("Component - InputField")] 
    [SerializeField] private TMP_InputField filePathInputField;
    [SerializeField] private TMP_InputField fileNameInputField;

    [Header("Component - Text")] 
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI fileDataText;

    [Header("Component - DropDown")] 
    [SerializeField] private TMP_Dropdown fileMode;
    [SerializeField] private TMP_Dropdown fileListDropDown;

    [Header("Default Option")] 
    [SerializeField] private string defaultPath = "/StreamingAssets/InitData/";
    
    private string _projectPath = string.Empty;
    private string _fullFilePath = string.Empty;
    
    private List<FileData> _fileList;

    private void Start()
    {
        InitGpt();
    }

    protected override void InitGpt()
    {
        base.InitGpt();
        
        filePathInputField.text = defaultPath;
        _fileList = new List<FileData>();
        
        RegisterFileUpdate();
    }

    public async void RegisterFile()
    {
        _projectPath = Application.dataPath + filePathInputField.text;
        _fullFilePath = _projectPath + fileNameInputField.text;

        if (CheckFileExists() == false)
        {
            resultText.text = $"{_fullFilePath}에 지정한 파일{fileNameInputField.text}(이/가) 존재하지 않습니다!!";
            return;
        }

        var mode = fileMode.options[fileMode.value].text;
        await Api.Files.UploadFileAsync(_fullFilePath, mode);
        
        resultText.text = $"{fileNameInputField.text}(이/가) 등록되었습니다.";
    }

    public async void ConvertFileToString()
    {
        var index = fileListDropDown.value;
        var fileData = await Api.Files.GetFileContentAsStringAsync(_fileList[index].ID);

        resultText.text = fileData;
    }

    public async void RegisterFileUpdate()
    {
        _fileList.Clear();
        fileListDropDown.options.Clear();
        
        var files = await Api.Files.GetFilesAsync();

        foreach (var file in files)
        {
            _fileList.Add(new FileData(file.Id, file.Name, file.Purpose, file.CreatedAt, file.Status));

            var newOption = new TMP_Dropdown.OptionData(file.Name);
            fileListDropDown.options.Add(newOption);
        }

        resultText.text = "등록 파일 불러오기 성공";
    }

    public async void DeleteFile()
    {
        var index = fileListDropDown.value;
        var fileID = _fileList[index].ID;

        await Api.Files.DeleteFileAsync(fileID);

        fileListDropDown.options.RemoveAt(index);
        fileListDropDown.RefreshShownValue();
        _fileList.RemoveAt(index);
        
        resultText.text = "파일 삭제 완료";
    }

    public void PrintFileData()
    {
        var fileData = _fileList[fileListDropDown.value];

        fileDataText.text = $"File ID : {fileData.ID} \n" +
                            $"File Name : {fileData.Name} \n" +
                            $"File Purpose : {fileData.Purpose} \n" +
                            $"File Date : {fileData.Date}\n" +
                            $"File Status : {fileData.Status}";
    }
    
    private bool CheckFileExists()
    {
        if (System.IO.File.Exists(_fullFilePath) == true)
        {
            return true;
        }

        return false;
    }
}

