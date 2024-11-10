using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManger : MonoBehaviour
{
    [Header("CrossHair")] 
    [SerializeField] private GameObject crossHair;
    [SerializeField] private GameObject headShotCrossHair;
    [SerializeField] private float delayTime = 0.02f;

    [Header("Magazine")] 
    [SerializeField] private GameObject magazineUI;
    [SerializeField] private TextMeshProUGUI curBulletCount;
    [SerializeField] private TextMeshProUGUI maxBulletCount;
    [SerializeField] private TextMeshProUGUI magCount;
    
    public bool MagazineUIEnable => magazineUI.activeSelf;
    
    #region Aim&Shoot State
    
    public void SetActiveCrossHair(bool enable, bool isHeadshot = false)
    {
        if (isHeadshot == true)
        {
            EnableHeadShotCrossHair().Forget();
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

    private async UniTask EnableHeadShotCrossHair()
    {
        headShotCrossHair.SetActive(true);

        await Task.Delay(TimeSpan.FromSeconds(delayTime));
        
        headShotCrossHair.SetActive(false);
    }
    
    #endregion
}
