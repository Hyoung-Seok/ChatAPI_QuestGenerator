using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

public class ExcelParser
{
    private FileStream _fileStream;
    private IExcelDataReader _fileReader;
    private DataTable _dataTable;
    private List<string> _keyList;
    
    private readonly int _columnsCount = 0;
    private readonly int _rowsCount = 0;
    
    public ExcelParser(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            throw new Exception("File not exists!");
        }

        _fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        _fileReader = ExcelReaderFactory.CreateReader(_fileStream);
        _dataTable = _fileReader.AsDataSet().Tables[0];
        _keyList = new List<string>();
        
        _columnsCount = _dataTable.Columns.Count;
        _rowsCount = _dataTable.Rows.Count;
    }

    /// <summary>
    /// 시트의 Key값 (row0)들을 가져옵니다.
    /// </summary>
    public List<string> GetKeyList()
    {
        for (var i = 0; i < _columnsCount; ++i)
        {
            _keyList.Add(_dataTable.Rows[0][i].ToString());
        }

        return _keyList;
    }

    /// <summary>
    /// Key 값에 있는 모든 Value값을 가져옵니다.
    /// </summary>
    public List<string> GetAllRowValue(string key)
    {
        var index = GetKeyColumnsIndex(key);
        var valueList = new List<string>();
        
        for (var row = 1; row < _rowsCount; ++row)
        {
            valueList.Add(_dataTable.Rows[row][index].ToString());
        }
        
        return valueList;
    }

    /// <summary>
    /// 선택한 Key, Value값을 string으로 변환합니다.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string ConvertSelectedDataToString(string key, string value)
    {
        var colIndex = GetKeyColumnsIndex(key);
        var rowIndex = -1;

        for (var row = 1; row < _rowsCount; ++row)
        {
            if (Equals(_dataTable.Rows[row][colIndex], value) == false)
            {
                continue;
            }

            rowIndex = row;
            break;
        }

        if (rowIndex == -1)
        {
            throw new Exception("Value Not exists!");
        }

        var values = new string[_columnsCount];
        
        for (var col = 0; col < _columnsCount; ++col)
        {
            values[col] = _dataTable.Rows[rowIndex][col].ToString();
        }

        var result = new StringBuilder();
        for (var i = 0; i < _keyList.Count; ++i)
        {
            if (i >= _keyList.Count - 1)
            {
                result.Append(_keyList[i] + " : " + values[i]);
                continue;
            }
            
            result.Append(_keyList[i] + " : " + values[i] + '\n');
        }
        

        return result.ToString();
    }
    
    private int GetKeyColumnsIndex(string key)
    {
        var index = _keyList.IndexOf(key);
        
        if (index < 0)
        {
            throw new Exception("Input Key is not exists!");
        }

        return index;
    }
}
