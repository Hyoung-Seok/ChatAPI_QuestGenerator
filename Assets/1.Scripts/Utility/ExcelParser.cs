using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using UnityEngine;

public class ExcelParser
{
    private int _baseRow;
    private int _rowCount;
    private int _columnCount;
    private List<string> _keyList;
    private ExcelWorksheet _workSheet;
    private ExcelPackage _package;

    public void InitParser(string path, int sheetIndex = 0, int baseRow = 1)
    {
        if (File.Exists(path) == false)
        {
            throw new Exception("File Not Exists!!");
        }

        var fileInfo = new FileInfo(path);
        _package = new ExcelPackage(fileInfo);
        _workSheet = _package.Workbook.Worksheets[sheetIndex];

        _baseRow = baseRow;
        _rowCount = _workSheet.Dimension.End.Row;
        _columnCount = _workSheet.Dimension.End.Column;
        
        InitKeyList();
    }

    // Key에 해당하는 모든 Value값 가져오기
    public List<string> GetAllBaseValue()
    {
        var result = new List<string>();

        for (var col = 2; col <= _columnCount; ++col)
        {
            result.Add(_workSheet.Cells[col, _baseRow].Text);
        }
        
        return result;
    }

    // Value가 존재하는 열 반환
    public int FindColumnWitValue(string value)
    {
        for (var col = 2; col <= _columnCount; ++col)
        {
            if (string.Equals(_workSheet.Cells[col, _baseRow].Text, value) == true)
            {
                return col;
            }
        }

        return -1;
    }

    // 해당 행의 모든 데이터를 string 형태로 반환
    public string ConvertValueDataToString(int col)
    {
        if (col <= 0)
        {
            return "Value Not Found!!";
        }
        
        var result = new StringBuilder();
        
        for (var row = 1; row <= _rowCount; ++row)
        {
            result.Append(_keyList[row - 1] + " : " + _workSheet.Cells[col, row].Text + '\n');
        }

        return result.ToString();
    }

    private void InitKeyList()
    {
        _keyList = new List<string>();

        for (var row = 1; row <= _rowCount; ++row)
        {
            _keyList.Add(_workSheet.Cells[1,row].Text);
        }
    }

}
