using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
using ExcelDataReader;
using TMPro;
using UnityEngine.UI;

public class DisplayExcelData : MonoBehaviour
{
    [Header("Path")] 
    [SerializeField] private string filePath = "_ExcelData/";
    [SerializeField] private string fileName = "NPCData.xlsx";

    [Header("Component")]
    [SerializeField] private TMP_Dropdown keyDropdown;
    [SerializeField] private TMP_Dropdown valueDropDown;
    [SerializeField] private Button loadValueBt;
    [SerializeField] private Button getPromptBt;
    [SerializeField] private TMP_Text resultData;
    
    private ExcelParser _excelParser;
    private string _data;
    
    private void Start()
    {
        try
        {
            _excelParser = new ExcelParser(Path.Combine(Application.dataPath + filePath + fileName));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        loadValueBt.interactable = true;
        InitKeyDropdown();
    }

    public void InitValueDropdown()
    {
        getPromptBt.interactable = true;
        valueDropDown.options.Clear();
        
        var values = _excelParser.GetAllRowValue(keyDropdown.options[keyDropdown.value].text);
        valueDropDown.AddOptions(values);
    }

    public void LoadSelectedData()
    {
        _data = _excelParser.ConvertSelectedDataToString(
            keyDropdown.options[keyDropdown.value].text,
            valueDropDown.options[valueDropDown.value].text);

        resultData.text = _data;
    }
    
    private void InitKeyDropdown()
    {
        var keys = _excelParser.GetKeyList();
        keyDropdown.AddOptions(keys);
    }
}
