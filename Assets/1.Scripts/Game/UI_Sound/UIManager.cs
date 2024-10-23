using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Animation Clip")] 
    [SerializeField] private Animator crossHairAnimation;

    private readonly int _enableKey = Animator.StringToHash("Enable");
    
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
}
