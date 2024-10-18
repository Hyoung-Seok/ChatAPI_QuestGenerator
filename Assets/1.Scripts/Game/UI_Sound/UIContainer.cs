using System.Collections.Generic;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    [Header("CrossHair")] 
    [SerializeField] private GameObject crossHair;

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
}
