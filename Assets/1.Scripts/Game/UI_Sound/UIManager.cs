using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("CrossHair")] 
    [SerializeField] private GameObject crossHair;

    [Header("Magazine")] 
    [SerializeField] private GameObject magazineUI;
    [SerializeField] private TextMeshProUGUI curBulletCount;
    [SerializeField] private TextMeshProUGUI maxBulletCount;
    [SerializeField] private TextMeshProUGUI magCount;

    [Header("Npc_UI")] 
    [SerializeField] private GameObject npcUI;
    [SerializeField] private GameObject questPrefabs;
    [SerializeField] private Transform questParent;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI npcField;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private List<Button> buttonList;
    [SerializeField] private List<Sprite> questStateSprite;
    [SerializeField] private int textSpeed = 200;

    [Header("Animation Clip")] 
    [SerializeField] private Animator crossHairAnimation;

    private string _defaultText;
    private Queue<QuestButton> _questListPool;
    private List<QuestButton> _currentQuestDisplay;
    private NpcController _curNpcController;
    private QuestButton _curSelectedQuestData;
    private IEnumerator _textPrintRoutine;
    
    private bool _isMagUIEnabled = false;
    private bool _isInputNextButton = false;
    private bool _isInputAcceptButton = false;
    private bool _isInputRefuseButton = false;
    
    private readonly int _enableKey = Animator.StringToHash("Enable");

    public void Init()
    {
        _questListPool = new Queue<QuestButton>();
        _currentQuestDisplay = new List<QuestButton>();
        
        CreateQuestDisplay(10);
        
        buttonList[(int)EButtonType.Next].onClick.AddListener(OnNextButtonClickEvent);
        buttonList[(int)EButtonType.Accept].onClick.AddListener(OnAcceptButtonClickEvent);
        buttonList[(int)EButtonType.Refuse].onClick.AddListener(OnRefuseButtonClickEvent);
    }
    
    #region NpcUI

    public void UpdateDefaultText(string text)
    {
        _defaultText = text;
        textField.text = _defaultText;
    }

    public void EnableNpcUI(List<QuestData> questData, NpcController controller)
    {
        _curNpcController = controller;
        
        _isMagUIEnabled = magazineUI.activeSelf;
        if(_isMagUIEnabled == true) magazineUI.SetActive(false);

        npcField.text = controller.NpcName;
        textField.text = _defaultText;
        npcUI.SetActive(true);
        
        GameManager.Instance.UnlockCursor();

        foreach (var data in questData)
        {
            GetQuestDisplay(data);
        }
    }

    public void DisableNpcUI()
    {
        ReturnToPoolQuestDisplay();
        _curSelectedQuestData = null;
        
        npcUI.SetActive(false);
        if(_isMagUIEnabled == true) magazineUI.SetActive(true);

        _curNpcController = null;
    }

    public Sprite GetQuestStateSprite(EQuestState state)
    {
        return questStateSprite[(int)state];
    }

    public void OnQuestClickEvent(int index)
    {
        _curSelectedQuestData = _currentQuestDisplay[index].GetComponent<QuestButton>();
        
        EnableButton(EButtonType.Next);
        questPanel.gameObject.SetActive(false);
        AllButtonActionValueDisable();

        switch (_curSelectedQuestData.QuestData.CurQuestState)
        {
            case EQuestState.Start:
                PrintStartTextRoutine().Forget();
                break;
            
            case EQuestState.Processing:
                PrintText(_curSelectedQuestData.QuestData.ScriptsData.ProcessScript).Forget();
                break;
            
            case EQuestState.Completion:
                PrintText(_curSelectedQuestData.QuestData.ScriptsData.ClearScript).Forget();
                
                ReturnToPoolQuestDisplay(index);
                _curNpcController.RemoveQuestData(index);
                GameManager.Instance.QuestManager.RemoveQuestData(_curSelectedQuestData.QuestData.Title);

                _curSelectedQuestData = null;
                break;
            
            default:
                return;
        }
    }

    private void EnableButton(EButtonType type)
    {
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
                break;
            
            default:
                return;
        }
    }

    private void OnNextButtonClickEvent()
    {
        _isInputNextButton = true;
    }

    private void OnAcceptButtonClickEvent()
    {
        _isInputAcceptButton = true;
    }

    private void OnRefuseButtonClickEvent()
    {
        _isInputRefuseButton = true;
    }
    
    private async UniTask PrintStartTextRoutine()
    {
        // 초기화
        var scripts = _curSelectedQuestData.QuestData.ScriptsData;
        textField.text = string.Empty;
        EnableButton(EButtonType.Next);
        
        // 스타트 텍스트 출력
        await PrintStartText(scripts.StartScripts);
        
        if (_isInputAcceptButton == true)
        {
            _curSelectedQuestData.UpdateQuestState(EQuestState.Processing);
            GameManager.Instance.QuestManager.AddQuest(_curSelectedQuestData.QuestData);
            
            await PrintText(scripts.AcceptScript);
        }
        else
        {
            await PrintText(scripts.RefuseScript);
        }

        await UniTask.WaitUntil(() => _isInputNextButton == true);
        
        questPanel.gameObject.SetActive(true);
        textField.text = _defaultText;
        EnableButton(EButtonType.None);
    }
    
    private async UniTask PrintStartText(List<string> scripts)
    {
        // 마지막 이전까지 대사 출력
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
        
        // 마지막 대사 출력
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
    }

    private async UniTask PrintText(string text)
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
        
        questPanel.gameObject.SetActive(true);
        textField.text = _defaultText;
        EnableButton(EButtonType.None);
    }

    private void AllButtonActionValueDisable()
    {
        _isInputNextButton = _isInputAcceptButton = _isInputRefuseButton = false;
    }

    private void CreateQuestDisplay(int createCount)
    {
        for (var i = 0; i < createCount; ++i)
        {
            var obj = Instantiate(questPrefabs, transform).GetComponent<QuestButton>();
            obj.gameObject.SetActive(false);
            _questListPool.Enqueue(obj);
        }
    }

    private void GetQuestDisplay(QuestData data)
    {
        if (_questListPool.Count <= 0)
        {
            CreateQuestDisplay(5);
        }

        var obj = _questListPool.Dequeue();
        obj.transform.SetParent(questParent);
        obj.SetQuestData(data);
        obj.gameObject.SetActive(true);
        
        _currentQuestDisplay.Add(obj);
    }

    private void ReturnToPoolQuestDisplay()
    {
        foreach (var quest in _currentQuestDisplay)
        {
            quest.ResetQuestData();
            quest.transform.SetParent(transform);
            quest.gameObject.SetActive(false);
            _questListPool.Enqueue(quest);
        }
        
        _currentQuestDisplay.Clear();
    }

    private void ReturnToPoolQuestDisplay(int index)
    {
        var obj = _currentQuestDisplay[index];
        _currentQuestDisplay.RemoveAt(index);
        
        obj.transform.SetParent(transform);
        obj.gameObject.SetActive(false);
        _questListPool.Enqueue(obj);
    }

    #endregion

    #region Aim&Shoot State
    
    public void SetActiveCrossHair(bool enable, bool isHeadshot = false)
    {
        if (isHeadshot == true)
        {
            crossHairAnimation.SetTrigger(_enableKey);
        }
        else
        {
            crossHair.SetActive(enable);
        }
    }

    public void SetActiveMagazineUI(bool enable)
    {
        magazineUI.SetActive(enable);
    }

    public void SetCurrentBulletInfoUI(int cur)
    {
        curBulletCount.text = cur.ToString();
    }

    public void SetMagazineCountUI(int mag)
    {
        magCount.text = mag.ToString();
    }

    public void SetMagazineInfoUI(int cur, int max)
    {
        curBulletCount.text = cur.ToString();
        maxBulletCount.text = max.ToString();
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
