using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using UnityEngine;

public class ExcelParser
{
    private static string FIRST_KEY = "QuestID";
    private const int COST_ROW = 10;
    
    private int _baseRow;
    private int _rowCount;
    private List<string> _keyList;
    private ExcelWorksheet _workSheet;
    private ExcelPackage _package;

    public ExcelParser(string path, int sheetIndex = 0, int baseRow = 1)
    {
        if (File.Exists(path) == false)
        {
            throw new Exception("File Not Exists!!");
        }

        var fileInfo = new FileInfo(path);
        _package = new ExcelPackage(fileInfo);
        _workSheet = _package.Workbook.Worksheets[sheetIndex];

        _baseRow = baseRow;
        
        CheckCurrentLowCount();
        InitKeyList();
    }

    // Key에 해당하는 모든 Value값 가져오기
    public List<string> GetAllValuesFromKey()
    {
        var result = new List<string>();
        var col = 2;

        while (string.IsNullOrEmpty(_workSheet.Cells[col, _baseRow].Text) == false)
        {
            result.Add(_workSheet.Cells[col++, _baseRow].Text);
        }
        
        return result;
    }

    // Value가 존재하는 열 반환
    public int FindColumnWitValue(string value)
    {
        var col = 2;
        while (string.IsNullOrEmpty(_workSheet.Cells[col, _baseRow].Text) == false)
        {
            if (string.Equals(_workSheet.Cells[col, _baseRow].Text, value) == true)
            {
                return col;
            }

            col++;
        }

        return -1;
    }

    // 해당 행의 모든 데이터를 string 형태로 반환
    public string ConvertValueDataToString(int col, bool ignoreFinalData = false)
    {
        if (col <= 0)
        {
            return "Value Not Found!!";
        }
        
        var result = new StringBuilder();
        var row = (ignoreFinalData) ? _rowCount - 1 : _rowCount;

        for (var i = 1; i < row; ++i)
        {
            result.Append(_keyList[i - 1] + " : " + _workSheet.Cells[col, i].Text + '\n');
        }
        
        return result.ToString();
    }
    
    // 해당 행의 모든 데이터를 List형태로 반환
    public List<string> ConvertValueDataToList(int col)
    {
        if (col <= 0)
        {
            return null;
        }

        var result = new List<string>();
        var row = 1;

        while (string.IsNullOrEmpty(_workSheet.Cells[col, row].Text) == false)
        {
            result.Add(_workSheet.Cells[col, row++].Text);
        }
        
        return result;
    }
    
    // 조건에 맞는 Value 모두 반환
    public List<string> GetValuesByLevel(int min, int max)
    {
        var curRow = -1;

        for (var row = 1; row < _rowCount; ++row)
        {
            if (int.TryParse(_workSheet.Cells[2, row].Text, out var temp) == true)
            {
                curRow = row;
            }
        }

        if (curRow < 0) return null;

        var result = new List<string>();
        var col = 2;

        while (string.IsNullOrEmpty(_workSheet.Cells[col, curRow].Text) == false)
        {
            var lv = int.Parse(_workSheet.Cells[col, curRow].Text);

            if (min <= lv && lv < max)
            {
                result.Add(_workSheet.Cells[col, _baseRow].Text);
            }

            col++;
        }

        return result;
    }
    
    // 엑셀에 데이터 저장
    public void SaveQuestDataInExcel(string data, string notice)
    {
        var inputCol = 2;
        
        while(string.IsNullOrEmpty(_workSheet.Cells[inputCol, 1].Text) == false)
        {
            inputCol++;
        }
        
        var valueList = GetValueInJsonString(data);

        foreach (var values in valueList)
        {
            for (var row = 1; row <= values.Count; ++row)
            {
                _workSheet.Cells[inputCol, row].Value = values[row - 1];
            }
            
            SaveGenerateCostData(inputCol);
            _workSheet.Cells[inputCol++, values.Count + 1].Value = notice;
        }
        
        _package.Save();
    }
    
    // 코스트 비용 저장.
    private void SaveGenerateCostData(int col)
    {
        var row = COST_ROW;
        
        for (var i = 0; i < TokensData.TokenData.Length; ++i)
        {
            _workSheet.Cells[col, row++].Value =
                $"Token : {TokensData.TokenData[i]} \n Cost : {TokensData.CostData[i]}$";
        }

        _workSheet.Cells[col, row].Value = $"{TokensData.CreateTime}S";
    }

    public static List<List<string>> GetValueInJsonString(string data)
    {
        var jObj = JObject.Parse(data);
        var result = new List<List<string>>();

        if (jObj[FIRST_KEY] != null)
        {
            var valueList = new List<string>();
            
            foreach (var value in jObj)
            {
                if (string.IsNullOrEmpty(value.ToString()) == true)
                {
                    valueList.Add("NULL");
                    continue;
                }
                
                valueList.Add(value.Value.ToString());
            }

            result.Add(valueList);

            return result;
        }

        foreach (var quest in jObj)
        {        
            var questValue = new List<string>();
            var questDetails = quest.Value as JObject;

            if (questDetails == null) continue;
            
            foreach (var value in questDetails)
            {           
                if (string.IsNullOrEmpty(value.ToString()) == true)
                {
                    questValue.Add("NULL");
                    continue;
                }

                questValue.Add(value.Value.ToString());
            }
            
            result.Add(questValue);
        }
        return result;
    }

    private void InitKeyList()
    {
        _keyList = new List<string>();

        for (var row = 1; row < _rowCount; ++row)
        {
            _keyList.Add(_workSheet.Cells[1, row].Text);
        }
    }

    private void CheckCurrentLowCount()
    {
        var row = 1;
        
        while (string.IsNullOrEmpty(_workSheet.Cells[1, row].Text) == false)
        {
            row++;
        }

        _rowCount = row;
    }

}
