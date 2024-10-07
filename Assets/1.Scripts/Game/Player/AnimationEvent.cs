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
}
