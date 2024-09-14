using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using UnityEngine;

public class ExcelWriter
{
    public List<string> KeyList => _keyList;
    
    private List<string> _keyList;
    private readonly string _filePath;
    
    private ExcelWorksheet _worksheet;
    private ExcelPackage _package;

    public ExcelWriter(string path)
    {
        if (File.Exists(path) == false)
        {
            throw new Exception("File Not Exists!!");
        }
        
        _filePath = path;
        
        var fileInfo = new FileInfo(_filePath);
        _package = new ExcelPackage(fileInfo);
        _worksheet = _package.Workbook.Worksheets[0];
        
        GetAllKeyValue();
    }

    // Write Excel Function
    public void WriteValueDataByKey(string key, int row, string value)
    {
        var colIndex = _keyList.IndexOf(key);

        if (colIndex < 0)
        {
            throw new Exception("Key Not Exists!!");
        }

        _worksheet.Cells[row, colIndex + 1].Value = value;
        _package.Save();
    }

    public void WriteValueDataByCol(int col, int row, string value)
    {
        _worksheet.Cells[row, col].Value = value;
        _package.Save();
    }

    public void WriteAllValueData(int row, List<string> valueList)
    {
        for (var col = 1; col <= valueList.Count; ++col)
        {
            _worksheet.Cells[row, col].Value = valueList[col - 1];
            _package.Save();
        }
    }
    
    private void GetAllKeyValue()
    {
        _keyList = new List<string>();
        var col = 1;

        while (string.IsNullOrEmpty(_worksheet.Cells[1, col].Text) == false)
        {
            _keyList.Add(_worksheet.Cells[1, col].Text);
            col++;
        }
    }
}
