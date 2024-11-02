using System;
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

    private Dictionary<string, QuestDisplay> _questDisplayViewDic;
    private string _npcName;
    private string _defaultText;
    private bool _isMagazineUIEnable;

    public event Action OnQuestAcceptClickEvent;
    public event Action OnQuestRefuseClickEvent;
    
    #region ButtonEvent

    private bool _isInputNextButton;
    private bool _isInputAcceptButton;
    private bool _isInputRefuseButton;
    
    #endregion

    public void Init()
    {
        _questDisplayViewDic = new Dictionary<string, QuestDisplay>();
        
        buttonList[(int)EButtonType.Next].onClick.AddListener(() => _isInputNextButton = true);
        buttonList[(int)EButtonType.Accept].onClick.AddListener(() => _isInputAcceptButton = true);
        buttonList[(int)EButtonType.Refuse].onClick.AddListener(() => _isInputRefuseButton = true);
    }

    public void EnableQuestDisplay(List<QuestData> data, string name, string text, Action<int> clickEvent)
    {
        textField.text = _defaultText = text;
        npcField.text = _npcName = name;
        
        questView.SetActive(true);
        processQuestPanel.SetActive(false);

        _isMagazineUIEnable = GameManager.Instance.PlayerUIManger.MagazineUIEnable;
        GameManager.Instance.PlayerUIManger.SetActiveMagazineUI(false);
        GameManager.Instance.SetCursorState(CursorLockMode.None);
        

        if (questContent.childCount != 0)
        {
            foreach (Transform child in questContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (var quest in data)
        {
            var obj = Instantiate(questButtonPrefabs, questContent).GetComponent<QuestButton>();
            
            obj.OnClickAction += clickEvent;
            obj.SetButtonData(quest);
        }
    }

    public void UpdateQuestButton(List<QuestData> data, Action<int> clickEvent)
    {
        if (questContent.childCount != 0)
        {
            foreach (Transform child in questContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (var quest in data)
        {
            var obj = Instantiate(questButtonPrefabs, questContent).GetComponent<QuestButton>();
            
            obj.OnClickAction += clickEvent;
            obj.SetButtonData(quest);
        }
    }

    public void RegisterProcessQuest(QuestData data)
    {
        var questDisplay = Instantiate(questTextPrefabs, questTextParent).GetComponent<QuestDisplay>();
        questDisplay.UpdateQuestDisplay(data.Title, data.TargetInfos, data.QuestType);
        
        _questDisplayViewDic.Add(data.Title, questDisplay);
    }

    public void UpdateProcessQuest(QuestData data)
    {
        if (_questDisplayViewDic.TryGetValue(data.Title, out var obj) == false)
        {
            return;
        }
        
        obj.UpdateQuestDisplay(data);
    }
    
    public void RemoveProcessQuest(string key)
    {
        if (_questDisplayViewDic.TryGetValue(key, out var display) == false)
        {
            return;
        }
        
        Destroy(display.gameObject);
        _questDisplayViewDic.Remove(key);
    }
    
    public void DisableQuestDisplay()
    {
        questView.SetActive(false);
        processQuestPanel.SetActive(true);

        if (_isMagazineUIEnable == true)
        {
            GameManager.Instance.PlayerUIManger.SetActiveMagazineUI(true);
        }
        
        GameManager.Instance.SetCursorState(CursorLockMode.Locked);
    }

    #region PrintText
    
    public async UniTask PrintText(List<string> scripts, bool enableAcceptBt)
    {
        textField.text = string.Empty;
        EnableButton(EButtonType.Next);
        questListPanel.SetActive(false);
        
        if (enableAcceptBt == true)
        {
            for (var i = 0; i < scripts.Count - 1; ++i)
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

                if (_isInputAcceptButton == true || _isInputRefuseButton == true)
                {
                    textField.text = scripts[^1];
                    _isInputRefuseButton = _isInputAcceptButton = false;
                    break;
                }

                await UniTask.Delay(textSpeed);
            }
            
            await UniTask.WaitUntil(() => _isInputAcceptButton == true || _isInputRefuseButton == true);
            
            textField.text = string.Empty;
            
            if (_isInputAcceptButton)
            {
                OnQuestAcceptClickEvent?.Invoke();
            }
            else if (_isInputRefuseButton)
            {
                OnQuestRefuseClickEvent?.Invoke();
            }
            
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
            _isInputNextButton = false;
            textField.text = string.Empty;
        }
        
        textField.text = string.Empty;
        EnableButton(EButtonType.None);
    }
    
    public async UniTask PrintText(string str)
    {
        questListPanel.SetActive(false);
        EnableButton(EButtonType.Next);
        textField.text = string.Empty;
        
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
        EnableButton(EButtonType.None);
    }
    
    #endregion
    
    private void EnableButton(EButtonType type)
    {
        _isInputNextButton = _isInputAcceptButton = _isInputRefuseButton = false;
        
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
    
    public Sprite GetQuestStateSprite(EQuestState state)
    {
        return questStateSprite[(int)state];
    }

}

public enum EButtonType
{
    Next,
    Accept,
    Refuse,
    None
}
