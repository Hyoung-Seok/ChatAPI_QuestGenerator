using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private RigController rigController;    
    
    [Header("WeaponChange")] 
    [SerializeField] private Transform backPos;
    [SerializeField] private Transform handPos;
    
    [Header("WeaponList")] 
    [SerializeField] private WeaponData weaponData;
    public WeaponData CurrentWeapon => weaponData;
    
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
