using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterDataDisplay : DisplayExcelData
{
    [Header("Component - MonsterData")] 
    [SerializeField] private TMP_InputField minLevel;
    [SerializeField] private TMP_InputField maxLevel;
    [SerializeField] private TMP_InputField additionalInfoField;

    private List<int> _conditionRow;
    private List<string> _sendMonsterName;
    
    private void Start()
    {
        InitData();
    }

    protected override void InitData()
    {
        base.InitData();

        _conditionRow = new List<int>();
        _sendMonsterName = new List<string>();
        
        Debug.Log("Monster Data Init Success!!");
    }

    public void GetConditionValue()
    {
        if (int.TryParse(minLevel.text, out var min) && int.TryParse(maxLevel.text, out var max) == true)
        {
            valueDropdown.options.Clear();
            _conditionRow.Clear();
            
            _conditionRow = ExcelReader.GetConditionRow(min, max);
            var valueList = new List<string>();

            for (var i = 0; i < _conditionRow.Count; ++i)
            {
                valueList.Add(ExcelReader.GetSelectedKeyRowData(_conditionRow[i], keyDropdown.value));
            }
            
            valueDropdown.AddOptions(valueList);
            return;
        }
        
        Debug.Log("올바른 값이 아닙니다.");
    }

    public void SaveCurrentMonster()
    {
        _sendMonsterName.Add(valueDropdown.options[valueDropdown.value].text);
        resultField.text = ConvertMonsterDataToString();
    }

    public string ConvertMonsterDataToString()
    {
        var result = string.Empty;
        
        foreach (var data in _sendMonsterName)
        {
            result += data + " , ";
        }

        return result + '\n' + additionalInfoField.text;
    }
}
