using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
    
    public ExcelParser(string filePath, FileMode fileMode = FileMode.Open, FileAccess fileAccess = FileAccess.Read, 
        FileShare fileShare = FileShare.Read)
    {
        if (File.Exists(filePath) == false)
        {
            throw new Exception("File not exists!");
        }

        _fileStream = File.Open(filePath, fileMode, fileAccess, fileShare);
        _fileReader = ExcelReaderFactory.CreateReader(_fileStream);
        _dataTable = _fileReader.AsDataSet().Tables[0];
        _keyList = new List<string>();
        
        _columnsCount = _dataTable.Columns.Count;
        _rowsCount = _dataTable.Rows.Count;
    }
    
    // 키 값 초기화
    public List<string> GetAllKeyValue()
    {
        _keyList.Clear();

        for (var col = 0; col < _columnsCount; ++col)
        {
            _keyList.Add(_dataTable.Rows[0][col].ToString());
        }

        return _keyList;
    }
    
    // 키 값에 해당하는 모든 Value값 불러오기
    public List<string> GetAllValueByKey(int col)
    {
        var result = new List<string>();

        for (var row = 1; row < _rowsCount; ++row)
        {
            result.Add(_dataTable.Rows[row][col].ToString());
        }

        return result;
    }
    
    // 선택된 key에서 value 찾기
    public string FindValueByKey(int col, string value)
    {
        for (var row = 1; row < _rowsCount; ++row)
        {
            if (string.Equals(_dataTable.Rows[row][col].ToString(), value) == true)
            {
                return ConvertRowDataToString(row);
            }
        }

        return "Can't Find value!!";
    }
    
    // 해당 행의 내용을 key + value 형태의 string으로 반환
    public string ConvertRowDataToString(int row)
    {
        var result = new StringBuilder();

        for (var col = 0; col < _columnsCount; ++col)
        {
            if (col >= _columnsCount - 1)
            {
                result.Append(_keyList[col] + " : " + _dataTable.Rows[row][col]);
                continue;
            }
            
            result.Append(_keyList[col] + " : " + _dataTable.Rows[row][col] + '\n');
        }

        return result.ToString();
    }
}
