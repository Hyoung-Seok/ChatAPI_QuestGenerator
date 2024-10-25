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
    [SerializeField] private TextMeshProUGUI npcField;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private List<Button> buttonList;
    [SerializeField] private List<Sprite> questStateSprite;
    [SerializeField] private int textSpeed = 200;

    [Header("Animation Clip")] 
    [SerializeField] private Animator crossHairAnimation;

    private Queue<QuestDisplay> _questListPool;
    private List<QuestDisplay> _currentQuestDisplay;
    private QuestData _curSelectedQuestData;
    private IEnumerator _textPrintRoutine;
    
    private bool _isMagUIEnabled = false;
    private bool _isInputNextButton = false;
    private bool _isInputAcceptButton = false;
    
    private readonly int _enableKey = Animator.StringToHash("Enable");

    public void Init()
    {
        _questListPool = new Queue<QuestDisplay>();
        _currentQuestDisplay = new List<QuestDisplay>();
        
        CreateQuestDisplay(10);
        
        buttonList[(int)EButtonType.Next].onClick.AddListener(OnNextButtonClickEvent);
        buttonList[(int)EButtonType.Accept].onClick.AddListener(OnAcceptButtonClickEvent);
        buttonList[(int)EButtonType.Refuse].onClick.AddListener(OnRefuseButtonClickEvent);
    }
    
    #region NpcUI

    public void EnableNpcUI(List<QuestData> questData)
    {
        _isMagUIEnabled = magazineUI.activeSelf;
        if(_isMagUIEnabled == true) magazineUI.SetActive(false);

        npcField.text = questData[0].NpcName;
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
    }

    public void OnQuestClickEvent(int index)
    {
        _curSelectedQuestData = _currentQuestDisplay[index].QuestData;
        
        EnableButton(EButtonType.Next);
        PrintTextRoutine().Forget();
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
            
            default:
                break;
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
        
    }
    
    private async UniTaskVoid PrintTextRoutine()
    {
        var scripts = _curSelectedQuestData.Scripts;
        var length = scripts.Count;
        var count = 0;
        
        textField.text = string.Empty;
        EnableButton(EButtonType.Next);

        while (true)
        {
            foreach (var c in scripts[count])
            {
                textField.text += c;
                
                if (_isInputNextButton == true)
                {
                    textField.text = scripts[count];
                    _isInputNextButton = false;
                    break;
                }
                
                await UniTask.Delay(textSpeed);
            }

            await UniTask.WaitUntil(() => _isInputNextButton == true);

            _isInputNextButton = false;
            textField.text = string.Empty;
            
            count++;
            if (count >= length - 1)
            {
                break;
            }
        }
        
        EnableButton(EButtonType.Accept);
        
        foreach (var c in scripts[count])
        {
            textField.text += c;

            if (_isInputAcceptButton == true)
            {
                textField.text = scripts[count];
                _isInputAcceptButton = false;
                break;
            }
            await UniTask.Delay(textSpeed);
        }

        await UniTask.WaitUntil(() => _isInputAcceptButton == true);
        
        //TODO : AcceptButton과 Refuse 버튼에 대한 상호작용 처리
        _isInputNextButton = false;
    }

    private void CreateQuestDisplay(int createCount)
    {
        for (var i = 0; i < createCount; ++i)
        {
            var obj = Instantiate(questPrefabs, transform).GetComponent<QuestDisplay>();
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
    Refuse
}
