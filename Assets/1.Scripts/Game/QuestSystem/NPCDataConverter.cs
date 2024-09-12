using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
using ExcelDataReader;
using TMPro;

public class NPCDataConverter : MonoBehaviour
{
    [Header("Path")] 
    [SerializeField] private string filePath = "_ExcelData/";
    [SerializeField] private string fileName = "NPCData.xlsx";

    [Header("Excel Info")] 
    [SerializeField] private int nameDataRow = 0;

    [Header("Component")] 
    [SerializeField] private TMP_Dropdown nameDropdown;
    [SerializeField] private TMP_Text resultText;

    private List<string> _nameList;
    private List<string> _keyList;
    private string _path = string.Empty;

    private FileStream _stream;
    private IExcelDataReader _reader;
    private DataSet _dataSet;
    private string _selectedNpcList;
    private string _resultData;

    private void Start()
    {
        if (FileExists() == false)
        {
            resultText.text = "File Not Exists";
        }
        
        InitFileStream();
        GetExcelKeyValue();
        GetAllNpcName();
    }

    public void InitFileStream()
    {
        _stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        _reader = ExcelReaderFactory.CreateReader(_stream);
        _dataSet = _reader.AsDataSet();
    }
    
    public void GetNpcData()
    {
        var npcName = nameDropdown.options[nameDropdown.value].text;
        var data = new List<string>();

        for (var columns = 1; columns < _dataSet.Tables[0].Columns.Count; columns++)
        {
            if (string.Equals(npcName, _dataSet.Tables[0].Rows[columns][nameDataRow].ToString()) ==
                false)
            {
                continue;
            }

            data = GetExcelColumnsData(columns);
            break;
        }

        _resultData = ConvertResultDataToString(data);
        resultText.text = _resultData;
    }
    
    private void GetAllNpcName()
    {
        _nameList = new List<string>();

        for (var columns = 1; columns < _dataSet.Tables[0].Columns.Count; columns++)
        {
            _nameList.Add(_dataSet.Tables[0].Rows[columns][nameDataRow].ToString());
        }
        
        nameDropdown.AddOptions(_nameList);
    }
    
    private List<string> GetExcelColumnsData(int columns)
    {
        var dataList = new List<string>();

        for (var row = 0; row < _dataSet.Tables[0].Rows.Count; row++)
        {
            dataList.Add(_dataSet.Tables[0].Rows[columns][row].ToString());
        }

        return dataList;
    }

    private void GetExcelKeyValue()
    {
        _keyList = new List<string>();

        for (var row = 0; row < _dataSet.Tables[0].Rows.Count; row++)
        {
            _keyList.Add(_dataSet.Tables[0].Rows[0][row].ToString());
        }
    }

    private string ConvertResultDataToString(List<string> dataList)
    {
        var result = new StringBuilder();

        for (var i = 0; i < _keyList.Count; ++i)
        {
            if (i >= _keyList.Count - 1)
            {
                result.Append(_keyList[i] + " : " + dataList[i]);
                continue;
            }
        
            result.Append(_keyList[i] + " : " + dataList[i] + '\n');
        }
        
        return result.ToString();
    }

    private bool FileExists()
    {
        _path = Path.Combine(Application.dataPath, filePath + fileName);
        return File.Exists(_path);
    }
}
