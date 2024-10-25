using System.Collections.Generic;
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

    [Header("Animation Clip")] 
    [SerializeField] private Animator crossHairAnimation;

    private Queue<QuestDisplay> _questListPool;
    private List<QuestDisplay> _currentQuestDisplay;
    
    private bool _isMagUIEnabled = false;
    private readonly int _enableKey = Animator.StringToHash("Enable");

    public void Init()
    {
        _questListPool = new Queue<QuestDisplay>();
        _currentQuestDisplay = new List<QuestDisplay>();
        
        CreateQuestDisplay(10);
    }
    
    #region NpcUI

    public void EnableNpcUI(List<QuestData> questData)
    {
        _isMagUIEnabled = magazineUI.activeSelf;
        if(_isMagUIEnabled == true) magazineUI.SetActive(false);
        
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
        
        npcUI.SetActive(false);
        if(_isMagUIEnabled == true) magazineUI.SetActive(true);
    }

    public void OnQuestClickEvent(int index)
    {
        Debug.Log(index);
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
