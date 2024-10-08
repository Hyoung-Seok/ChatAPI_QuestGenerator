using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimationEvent : MonoBehaviour
{
    [Header("WeaponChange")] 
    [SerializeField] private Transform backPos;
    [SerializeField] private Transform handPos;
    
    [Header("WeaponList")] 
    [SerializeField] private WeaponData weaponData;

    [SerializeField] private Transform test;

    [Header("Debug")] 
    [SerializeField] private bool enableIK;
    
    private Animator _playerAnimator;
    
    private void Start()
    {
        _playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
    }

    public void EquipWeapon()
    {
        var obj = backPos.GetChild(0);
        obj.SetParent(handPos);

        obj.localPosition = weaponData.OnHandPosition;
        obj.localRotation = Quaternion.Euler(weaponData.OnHandRotation);
        obj.localScale = weaponData.OnHandScale;
    }

    public void UnEquipWeapon()
    {
        var obj = handPos.GetChild(0);
        obj.SetParent(backPos);
        
        obj.localPosition = weaponData.OnBackPosition;
        obj.localRotation = Quaternion.Euler(weaponData.OnBackRotation);
        obj.localScale = weaponData.OnBackScale;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableIK == false)
        {
            return;
        }
        
        if (_playerAnimator)
        {
            _playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
            _playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.5f);
            
            _playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, test.position);
            _playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, test.rotation);
            
            return;
        }
        
        Debug.Log("false");
    }
}
