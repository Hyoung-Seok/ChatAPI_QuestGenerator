using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class ExcelParser
{
    private static string FIRST_KEY = "QuestID";
    private const int COST_ROW = 10;
    
    private int _baseRow;
    private int _rowCount;
    private List<string> _keyList;

    private string _path;
    private IWorkbook _workbook;
    private ISheet _sheet;

    public ExcelParser(string path, int sheetIndex = 0)
    {
        if (File.Exists(path) == false)
        {
            throw new Exception("File Not Exists!!");
        }

        _path = path;
        var fileStream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
        _workbook = new XSSFWorkbook(fileStream);
        _sheet = _workbook.GetSheetAt(sheetIndex);
        
        CheckMaxRowCount();
        InitKeyList();
    }

    // Key에 해당하는 모든 Value값 가져오기
    public List<string> GetAllValuesFromKeyOrNull(string key = "Name")
    {
        var result = new List<string>();
        var keyIndex = _keyList.IndexOf(key);

        if (keyIndex <= -1)
        {
            return null;
        }

        var index = 1;
        var row = _sheet.GetRow(index);
        var cell = row.GetCell(keyIndex);

        while (string.IsNullOrEmpty(cell?.ToString()) == false)
        {
            result.Add(cell.ToString());
            
            row = _sheet.GetRow(++index);
            cell = row?.GetCell(keyIndex);
        }
        
        return result;
    }

    // Value가 존재하는 열 반환
    public int FindColumnWithValue(string value, string key = "Name")
    {
        var keyIndex = _keyList.IndexOf(key);

        var index = 1;
        var row = _sheet.GetRow(index);
        var cell = row.GetCell(keyIndex);
        
        while (string.IsNullOrEmpty(cell?.ToString()) == false)
        {
            if (string.Equals(cell?.ToString(), value) == true)
            {
                return index;
            }

            row = _sheet.GetRow(++index);
            cell = row.GetCell(keyIndex);
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
        
        var rowCell = _sheet.GetRow(col);
        var iter = (ignoreFinalData) ? _rowCount - 1 : _rowCount;
        
        for (var row = 0; row < iter; ++row)
        {
            var cell = rowCell.GetCell(row);
            result.Append(_keyList[row] + " : " + cell.ToString() + '\n');
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
        var rowCell = _sheet.GetRow(col);

        for (var row = 0; row < _rowCount; ++row)
        {
            var cell = rowCell.GetCell(row);
            result.Add(cell.ToString());
        }

        return result;
    }
    
    // 조건에 맞는 Value 모두 반환
    public List<string> GetValuesByLevel(int min, int max)
    {
        var levelIndex = _keyList.IndexOf("LV");
        
        if (levelIndex < 0) return null;

        var result = new List<string>();
        var nameIndex = _keyList.IndexOf("Name");

        var index = 1;
        var rowCell = _sheet.GetRow(index);
        var cell = rowCell.GetCell(levelIndex);
            
        while (string.IsNullOrEmpty(cell?.ToString()) == false)
        {
            var value = cell.ToString();
            var lv = int.Parse(value);
        
            if (min <= lv && lv < max)
            {
                var nameRow = _sheet.GetRow(index);
                var nameCell = nameRow.GetCell(nameIndex);
                
                result.Add(nameCell.ToString());
            }
        
            rowCell = _sheet.GetRow(++index);
            cell = rowCell?.GetCell(levelIndex);
        }

        return result;
    }
    
    // 엑셀에 데이터 저장
    public void SaveQuestDataInExcel(string data, string notice)
    {
        var lastRowIndex = _sheet.LastRowNum + 1;
        var valueList = GetValueInJsonString(data);
        var newRow = _sheet.CreateRow(lastRowIndex);
        var count = 1;
        
        foreach (var value in valueList)
        {
            for (var i = 0; i < value.Count; ++i)
            {
                var cell = newRow.CreateCell(i);
                cell.SetCellValue(value[i]);
            }

            var noticeCell = newRow.CreateCell(value.Count);
            noticeCell.SetCellValue(notice);
            
            SaveGenerateCostData(newRow);
            
            newRow = _sheet.CreateRow(lastRowIndex + count);
            count++;
        }

        using var writeStream = new FileStream(_path, FileMode.Create, FileAccess.Write);
        
        _workbook.Write(writeStream);
        writeStream.Close();
    }
    
    // 코스트 비용 저장.
    private void SaveGenerateCostData(IRow rowCell)
    {
        for (var i = 0; i < TokensData.TokenData.Length; ++i)
        {
            var data = $"Token : {TokensData.TokenData[i]} \n Cost : {TokensData.CostData[i]}$";

            var newCell = rowCell.CreateCell(COST_ROW + i);
            newCell.SetCellValue(data);
        }
        
        var timeCell = rowCell.CreateCell(COST_ROW + TokensData.TokenData.Length);
        timeCell.SetCellValue($"{TokensData.CreateTime}S");
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
        var rowData = _sheet.GetRow(0);

        for (var row = 0; row < _rowCount; ++row)
        {
            var cell = rowData.GetCell(row);
            _keyList.Add(cell?.ToString());
        }
    }

    private void CheckMaxRowCount()
    {
        var row = _sheet.GetRow(0);
        var cell = row.GetCell(0);

        while (string.IsNullOrEmpty(cell?.StringCellValue) == false)
        {
            cell = row.GetCell(_rowCount++);
        }

        _rowCount--;
    }
}
