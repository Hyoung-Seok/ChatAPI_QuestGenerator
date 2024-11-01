using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [Header("QuestViewUI")] 
    [SerializeField] private GameObject questView;
    [SerializeField] private GameObject questButtonPrefabs;
    [SerializeField] private Transform questContent;
    [SerializeField] private GameObject questListPanel;
    [SerializeField] private TextMeshProUGUI npcField;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private List<Button> buttonList;
    [SerializeField] private List<Sprite> questStateSprite;
    [SerializeField] private int textSpeed = 80;

    [Header("ProcessQuestUI")] 
    [SerializeField] private GameObject processQuestPanel;
    [SerializeField] private GameObject questTextPrefabs;
    [SerializeField] private Transform questTextParent;
    
    private Queue<QuestButton> _questButtonPool;
    private List<QuestButton> _curDisplayButton;
    private List<QuestData> _curDisplayQuest;
    private QuestManager _questManager;
    
    private string _curInteractionNpcName;
    private string _defaultText;
    private bool _isMagazineUIEnable;

    #region ButtonEvent

    private bool _isInputNextButton;
    private bool _isInputAcceptButton;
    private bool _isInputRefuseButton;
    
    #endregion

    public void Init()
    {
        _questButtonPool = new Queue<QuestButton>();
        _curDisplayButton = new List<QuestButton>();
        
        CreateQuestButton(10);
        
        buttonList[(int)EButtonType.Next].onClick.AddListener(() => _isInputNextButton = true);
        buttonList[(int)EButtonType.Accept].onClick.AddListener(() => _isInputAcceptButton = true);
        buttonList[(int)EButtonType.Refuse].onClick.AddListener(() => _isInputRefuseButton = true);

        _questManager = GameManager.Instance.QuestManager;
    }
    
    public void EnableQuestDisplay(List<QuestData> data, string npcName, string defaultText)
    {
        _curDisplayQuest = data;
        npcField.text = _curInteractionNpcName = npcName;
        textField.text = _defaultText = defaultText;
        questView.SetActive(true);
        
        GameManager.Instance.UnlockCursor();

        _isMagazineUIEnable = GameManager.Instance.PlayerUIManger.MagazineUIEnable;
        GameManager.Instance.PlayerUIManger.SetActiveMagazineUI(false);
        
        if (data.Count <= 0)
        {
            return;
        }

        foreach (var quest in _curDisplayQuest)
        {
            GetQuestButton(quest);
        }
    }

    public void DisableQuestDisplay()
    {
        _curInteractionNpcName = string.Empty;
        _curDisplayQuest.Clear();
        questView.SetActive(false);
        
        ReturnQuestButtonToPool();
        GameManager.Instance.PlayerUIManger.SetActiveMagazineUI(_isMagazineUIEnable);
    }

    public void UpdateQuestPanel(List<QuestData> data)
    {
        _curDisplayQuest.Clear();
        _curDisplayQuest = data;
        
        if (data.Count <= 0)
        {
            return;
        }

        foreach (var quest in _curDisplayQuest)
        {
            GetQuestButton(quest);
        }
    }
    
    private void RegisterProcessQuest(QuestData data)
    {
        data.CurQuestState = EQuestState.Processing;
        
        var obj = Instantiate(questTextPrefabs, questTextParent).GetComponent<QuestDisplay>();
        obj.UpdateQuestDisplay(data.Title, data.TargetInfos, data.QuestType);
        
        _questManager.CurQuestDisplay.Add(obj);
    }

    public void DeregisterProcessQuest(string key)
    {
        var questDisplay = _questManager.CurQuestDisplay.FirstOrDefault(x => string.Equals(x.Title, key));

        if (questDisplay == null)
        {
            return;
        }
        
        _questManager.CurQuestDisplay.Remove(questDisplay);
        Destroy(questDisplay.gameObject);
    }

    private async void OnQuestButtonClickEvent(int index)
    {
        Debug.Log(index);
        var data = _curDisplayQuest[index];
        EnableButton(EButtonType.Next);
        questListPanel.SetActive(false);

        switch (data.CurQuestState)
        {
            case EQuestState.Start:
                await PrintScriptsListTextRoutine(data.ScriptsData.StartScripts, true);
                break;
            
            case EQuestState.Processing:
                await PrintStringTextRoutine(data.ScriptsData.ProcessScript);
                break;
            
            case EQuestState.Completion:
                await PrintScriptsListTextRoutine(data.ScriptsData.ClearScript, false);
                
                DeregisterProcessQuest(data.Title);
                ReturnToPoolQuestDisplay(index);
                _questManager.RemoveClearQuest(index, data);
                break;
            
            default:
                return;
        }

        if (_isInputAcceptButton == true)
        {
            await PrintStringTextRoutine(data.ScriptsData.AcceptScript);
            
            if (data.QuestType == EQuestType.Deliver)
            {
                GameManager.Instance.QuestManager.CurInteractionNpc.SetDeliverQuestData(data);
            }
       
            RegisterProcessQuest(_curDisplayQuest[index]);
            questContent.GetChild(index).GetComponent<QuestButton>().UpdateButtonState(EQuestState.Processing);
                
            _questManager.CurrentProcessQuest.Add(data);   
        }

        if (_isInputRefuseButton == true)
        {
            await PrintStringTextRoutine(data.ScriptsData.RefuseScript);
        }
        
        EnableButton(EButtonType.None);
    }

    public Sprite GetQuestStateSprite(EQuestState state)
    {
        return questStateSprite[(int)state];
    }
    
    #region PrintText
    
    private async UniTask PrintScriptsListTextRoutine(IReadOnlyList<string> scripts, bool enableAcceptButton)
    {
        textField.text = string.Empty;
        
        if (enableAcceptButton == true)
        {
            for(var i = 0; i < scripts.Count - 1; ++i)
            {
                foreach (var c in scripts[i])
                {
                    textField.text += c;

                    if (_isInputNextButton == true)
                    {
                        textField.text = scripts[i];
                        _isInputNextButton = false;
                        break;
                    }

                    await UniTask.Delay(textSpeed);
                }

                await UniTask.WaitUntil(() => _isInputNextButton == true);

                _isInputNextButton = false;
                textField.text = string.Empty;
            }
            
            EnableButton(EButtonType.Accept);
            foreach (var c in scripts[^1])
            {
                textField.text += c;

                if (_isInputAcceptButton || _isInputRefuseButton)
                {
                    textField.text = scripts[^1];
                    _isInputAcceptButton = _isInputRefuseButton = false;
                    break;
                }

                await UniTask.Delay(textSpeed);
            }
        
            await UniTask.WaitUntil(() => _isInputAcceptButton || _isInputRefuseButton);
            
            textField.text = string.Empty;
            
            return;
        }
        
        foreach (var str in scripts)
        {
            foreach (var c in str)
            {
                textField.text += c;

                if (_isInputNextButton == true)
                {
                    textField.text = str;
                    _isInputNextButton = false;
                    break;
                }

                await UniTask.Delay(textSpeed);
            }
            
            await UniTask.WaitUntil(() => _isInputNextButton == true);
        
            textField.text = string.Empty;
            _isInputNextButton = false;
        }
    }

    private async UniTask PrintStringTextRoutine(string text)
    {
        textField.text = string.Empty;
        EnableButton(EButtonType.Next);

        foreach (var c in text)
        {
            textField.text += c;

            if (_isInputNextButton)
            {
                textField.text = text;
                _isInputNextButton = false;
                break;
            }

            await UniTask.Delay(textSpeed);
        }
        
        await UniTask.WaitUntil(() => _isInputNextButton == true);
        
        _isInputNextButton = false;
        textField.text = string.Empty;
    }
    
    #endregion
    
    private void AllButtonValueDisable()
    {
        _isInputNextButton = _isInputAcceptButton = _isInputRefuseButton = false;
    }
    
    private void EnableButton(EButtonType type)
    {
        AllButtonValueDisable();
        
        switch (type)
        {
            case EButtonType.Next:
                buttonList[0].gameObject.SetActive(true);
                
                buttonList[1].gameObject.SetActive(false);
                buttonList[2].gameObject.SetActive(false);
                break;
            
            case EButtonType.Accept:
            case EButtonType.Refuse:
                buttonList[0].gameObject.SetActive(false);
                
                buttonList[1].gameObject.SetActive(true);
                buttonList[2].gameObject.SetActive(true);
                break;
            
            case EButtonType.None:
                buttonList.ForEach(x => x.gameObject.SetActive(false));

                questListPanel.gameObject.SetActive(true);
                textField.text = _defaultText;
                break;
            
            default:
                return;
        }
    }
    

    #region ObjectPoolFunction

    private void CreateQuestButton(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            var obj = Instantiate(questButtonPrefabs, transform).GetComponent<QuestButton>();
            obj.gameObject.SetActive(false);
            _questButtonPool.Enqueue(obj);
        }
    }

    private void GetQuestButton(QuestData data)
    {
        if (_questButtonPool.Count < 0)
        {
            CreateQuestButton(5);
        }

        var obj = _questButtonPool.Dequeue();
        obj.SetButtonData(data);
        obj.OnClickAction += OnQuestButtonClickEvent;
        
        obj.gameObject.transform.SetParent(questContent);
        obj.gameObject.SetActive(true);
        
        _curDisplayButton.Add(obj);
    }

    private void ReturnQuestButtonToPool()
    {
        foreach (Transform child in questContent)
        {
            var button = child.GetComponent<QuestButton>();
            button.ResetButtonData();
            button.gameObject.transform.SetParent(transform);
            button.gameObject.SetActive(false);
            
            _questButtonPool.Enqueue(button);
        }
        
        _curDisplayButton.Clear();
    }
    
    private void ReturnToPoolQuestDisplay(int index)
    {
        var obj = _curDisplayButton[index];
        _curDisplayQuest.RemoveAt(index);
        
        obj.ResetButtonData();
        obj.transform.SetParent(transform);
        obj.gameObject.SetActive(false);
        
        _questButtonPool.Enqueue(obj);
    }

    #endregion
}

public enum EButtonType
{
    Next,
    Accept,
    Refuse,
    None
}
