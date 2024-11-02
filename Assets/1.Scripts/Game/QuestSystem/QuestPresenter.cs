using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QuestPresenter
{
    private QuestUIManager _questUIManager;
    private QuestManager _questManager;
    private int _index = 0;
    
    public QuestPresenter(QuestUIManager uiManager, QuestManager questManager)
    {
        _questUIManager = uiManager;
        _questManager = questManager;

        _questManager.UpdateProcessQuest += UpdateProcessQuestView;
    }

    public void Init()
    {
        _questManager.CurInteractionNpc.OnEnableQuestUIAction += EnableQuestUI;
        _questManager.CurInteractionNpc.OnDisableQuestUIAction += DisableQuestUI;

        _questUIManager.OnQuestAcceptClickEvent += OnQuestAcceptClickEvent;
        _questUIManager.OnQuestRefuseClickEvent += OnQuestRefuseClickEvent;
    }
    
    public void Clear()
    {
        if (_questManager.CurInteractionNpc == null)
        {
            return;
        }
        
        _questManager.CurInteractionNpc.OnEnableQuestUIAction -= EnableQuestUI;
        _questManager.CurInteractionNpc.OnDisableQuestUIAction -= DisableQuestUI;
        
        _questUIManager.OnQuestAcceptClickEvent -= OnQuestAcceptClickEvent;
        _questUIManager.OnQuestRefuseClickEvent -= OnQuestRefuseClickEvent;
        
        _index = -1;
    }

    private void EnableQuestUI()
    {
        _questUIManager.EnableQuestDisplay(_questManager.CurNpcQuest, _questManager.CurInteractionNpc.NpcName,
            _questManager.CurInteractionNpc.DefaultText, OnQuestClickEvent);
    }

    private void DisableQuestUI()
    {
        _questUIManager.DisableQuestDisplay();
    }

    private void OnQuestClickEvent(int index)
    {
        _index = index;
        var quest = _questManager.CurNpcQuest[_index];
        
        switch (quest.CurQuestState)
        {
            case EQuestState.Start:
                 _questUIManager.PrintText(quest.ScriptsData.StartScripts, true).Forget();
                break;
            
            case EQuestState.Processing:
                 _questUIManager.PrintText(quest.ScriptsData.ProcessScript).Forget();
                break;
            
            case EQuestState.Completion:
                 _questUIManager.PrintText(quest.ScriptsData.ClearScript, false).Forget();
                RemoveClearQuest(quest);
                break;
            
            default:
                return;
        }
    }

    private void OnQuestAcceptClickEvent()
    {
        var quest = _questManager.CurNpcQuest[_index];
        _questUIManager.PrintText(quest.ScriptsData.AcceptScript).Forget();

        if (quest.QuestType == EQuestType.Deliver)
        {
            _questManager.SetDeliverQuest(_index);
        }

        _questManager.UpdateQuest(_index, EQuestState.Processing);
        _questUIManager.UpdateQuestButton(_questManager.CurNpcQuest, OnQuestClickEvent);
        _questUIManager.RegisterProcessQuest(quest);
    }

    private void OnQuestRefuseClickEvent()
    {
        var quest = _questManager.CurNpcQuest[_index];
        _questUIManager.PrintText(quest.ScriptsData.RefuseScript).Forget();
    }

    private void UpdateProcessQuestView(QuestData data)
    {
        _questUIManager.UpdateProcessQuest(data);
    }

    private void RemoveClearQuest(QuestData data)
    {
        while (data != null)
        {
            if (_questManager.RemoveClearQuest(data) == false)
            {
                return;
            }
        
            _questUIManager.RemoveProcessQuest(data.Title);
            _questUIManager.UpdateQuestButton(_questManager.CurNpcQuest, OnQuestClickEvent);

            if (data.ChainQuest.Value == default)
            {
                break;
            }
        
            var pair = data.ChainQuest;
            var target = GameManager.Instance.NpcManager.GetNpcControllerOrNull(data.ChainQuest.Value.NpcName);

            target.RemoveQuestData(pair.Value);
            data = pair.Value;
        }
    }
}
