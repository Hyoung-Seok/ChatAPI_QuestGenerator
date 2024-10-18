using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIContainer : MonoBehaviour
{
    [Header("CrossHair")] 
    [SerializeField] private GameObject crossHair;
    [SerializeField] private GameObject headShotCrossHair;

    public void SetActiveCrossHair(bool enable, bool isHeadshot = false)
    {
        if (isHeadshot == true)
        {
            headShotCrossHair.SetActive(enable);
        }
        else
        {
            crossHair.SetActive(enable);
        }
    }
}
