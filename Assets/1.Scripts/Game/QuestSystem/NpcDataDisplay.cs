using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcDataDisplay : DisplayExcelData
{
    [Header("Component - NPC Display")] 
    [SerializeField] private TMP_Dropdown questType;
    [SerializeField] private TMP_InputField additionalInfoField;

    public string NpcData => _npcData;
    public string AddInfo => additionalInfoField.text;
    
    private string _npcData;
    
    private void Start()
    {
        InitData();
    }

    public void SaveSelectedData()
    {
        _npcData = ConvertSelectedDataToString();
        _npcData += '\n' + "Type : " + questType.options[questType.value].text;

        if (additionalInfoField.text != string.Empty)
        {
            _npcData += '\n' + "Apprise : " + additionalInfoField.text;
        }
        
        resultField.text = _npcData;
    }

    public void SaveSearchData()
    {
        _npcData = ConvertSearchDataToString();
        _npcData += '\n' + "Type : " + questType.options[questType.value].text;
        
        if (additionalInfoField.text != string.Empty)
        {
            _npcData += '\n'+ "Apprise : " + additionalInfoField.text;
        }

        resultField.text = _npcData;
    }

    protected override void InitData()
    {
        base.InitData();
        
        Debug.Log("NPC Display Init Success!");
    }
}
