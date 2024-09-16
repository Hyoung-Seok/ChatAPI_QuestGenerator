using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject questButton;
    [SerializeField] private GameObject interactionText;
    [SerializeField] private GameObject nextBt;
    [SerializeField] private GameObject accBt;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scriptsText;
    [SerializeField] private PlayerController playerController;

    public static QuestUIManager Instance => _questUIManager;
    public bool CurrentState => UI.activeSelf;
    
    private static QuestUIManager _questUIManager;
    private List<Button> _curQuestButton;

    private string[] _curScripts;
    private int _curScriptsIndex = 0;

    private void Awake()
    {
        if (_questUIManager == null)
        {
            _questUIManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        
        _curQuestButton = new List<Button>();
        CreateQuestListButton(5);
        
        UI.SetActive(false);
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
            _curQuestButton[i].onClick.AddListener(questData[i].OnClickEvent);
        }
    }

    public void InitNpcTextInfo(string npcName, string text)
    {
        playerController.ChangeMainState(playerController.IdleState);
        
        UI.SetActive(true);
        interactionText.SetActive(false);
        
        nameText.text = npcName;
        scriptsText.text = text;
    }

    public void InitScripts(List<string> scripts)
    {
        _curScripts = scripts.ToArray();
        _curScriptsIndex = 0;
        
        PrintScripts();
    }

    public void PrintScripts()
    {
        nextBt.SetActive(true);
        
        if (_curScriptsIndex < _curScripts.Length - 1)
        {
            scriptsText.text = _curScripts[_curScriptsIndex++];
            return;
        }

        nextBt.SetActive(false);
        accBt.SetActive(true);
        
        scriptsText.text = _curScripts[_curScriptsIndex];
    }

    public void OnAcceptEvent()
    {
        playerController.ChangeMainState(playerController.MoveState);
        
        accBt.SetActive(false);
        UI.SetActive(false);
    }
    
    public void SetInteractionButton(bool active)
    {
        interactionText.SetActive(active);    
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
