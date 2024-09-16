using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject questButton;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scriptsText;

    public static QuestManager Instance => _questManager;
    public bool CurrentState => gameObject.activeSelf;
    
    private static QuestManager _questManager;
    private List<Button> _curQuestButton;

    private void Awake()
    {
        if (_questManager == null)
        {
            _questManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        
        _curQuestButton = new List<Button>();
        CreateQuestListButton(5);
        
        gameObject.SetActive(false);
    }
    

    public void InitQuestButton(List<QuestData> questData)
    {
        var count = _curQuestButton.Count - questData.Count;

        if (count < 0)
        {
            CreateQuestListButton(Mathf.Abs(count));
        }

        for (var i = 0; i < questData.Count; ++i)
        {
            _curQuestButton[i].GetComponentInChildren<TMP_Text>().text = questData[i].Title;
            _curQuestButton[i].gameObject.SetActive(true);
        }
    }

    public void InitNpcTextInfo(string npcName, string text)
    {
        gameObject.SetActive(true);
        
        nameText.text = npcName;
        scriptsText.text = text;
    }

    private void CreateQuestListButton(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            var obj = Instantiate(questButton, buttonParent.transform);
            obj.SetActive(false);
        
            _curQuestButton.Add(obj.GetComponent<Button>());
        }
    }
}
