using System;
using System.IO;
using UnityEngine;
using TMPro;

public class DisplayExcelData : MonoBehaviour
{
    [Header("Path")] 
    [SerializeField] private string filePath = "/_ExcelData/";
    [SerializeField] private string fileName = "NPCData.xlsx";

    [Header("Component - Default")] 
    [SerializeField] private TMP_Dropdown keyDropdown;
    [SerializeField] private TMP_Dropdown valueDropdown;
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] protected TMP_Text resultField;
    
    protected ExcelParser ExcelParser;
    
    protected virtual void InitData()
    {
        try
        {
            ExcelParser = new ExcelParser(Path.Combine(Application.dataPath + filePath + fileName));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        InitKeyDropdown();
    }

    // 키 드랍다운 초기화
    private void InitKeyDropdown()
    {
        keyDropdown.AddOptions(ExcelParser.GetAllKeyValue());
    }
    
    // 키 값에 해당하는 모든 Value초기화
    public void InitValueDropdown()
    {
        valueDropdown.options.Clear();
        valueDropdown.AddOptions(ExcelParser.GetAllValueByKey(keyDropdown.value));
    }
    
    // 현재 선택된 값 저장
    protected string ConvertSelectedDataToString()
    {
        return ExcelParser.ConvertRowDataToString(valueDropdown.value + 1);
    }

    // 키 값에 해당하는 value값 검색 후 저장
    protected string ConvertSearchDataToString()
    {
        return ExcelParser.FindValueByKey(keyDropdown.value, searchInputField.text);
    }
}
