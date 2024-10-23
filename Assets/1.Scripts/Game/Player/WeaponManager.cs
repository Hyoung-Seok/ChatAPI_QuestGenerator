using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private MultiAimConstraint bodyAim;
    [SerializeField] private MultiAimConstraint rightHandAim;
    [SerializeField] private TwoBoneIKConstraint leftHandGrab;

    [Header("Weight")] 
    [SerializeField] private float maxWeight = 0.7f;

    [Header("TwoBonePosition")] 
    [SerializeField] private Transform grabPosition;
    [SerializeField] private Transform hintPosition;

    [Header("Weapon")] 
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Transform backPos;
    [SerializeField] private Transform handPos;
    
    public bool IsReloading { get; private set; }
    public bool CanReloading => currentWeapon.CanReload;
    private WeaponData _weaponData;
    private IEnumerator _setWeightRoutine;
    private IEnumerator _reloadRoutine;

    private void Start()
    {
        _weaponData = currentWeapon.WeaponData;
        currentWeapon.enabled = false;
    }

    public void ChangeWeaponState(EPlayerInputState state)
    {
        switch (state)
        {
            case EPlayerInputState.IDLE:
                rightHandAim.weight = bodyAim.weight = 0;
                
                SetWeaponTransform(_weaponData.HandTf);
                SetWeaponGrabPoint(_weaponData.IdleTargetPoint, _weaponData.IdleHintPoint);

                currentWeapon.enabled = false;
                GameManager.Instance.CameraEffect.TransitionCamera(EPlayerInputState.IDLE);
                break;
            
            case EPlayerInputState.AIM:
                SetWeaponTransform(_weaponData.AimTf);
                SetWeaponGrabPoint(_weaponData.AimTargetPoint, _weaponData.AimHintPoint);
                
                bodyAim.weight = maxWeight;
                rightHandAim.weight = 1.0f;

                currentWeapon.enabled = true;
                GameManager.Instance.CameraEffect.TransitionCamera(EPlayerInputState.AIM);
                break;
            
            default:
                return;
        }
    }

    private void SetWeaponTransform(WeaponTransform tf)
    {
        var weaponTf = currentWeapon.transform;
        
        weaponTf.SetLocalPositionAndRotation(tf.Position, Quaternion.Euler(tf.Rotation));
        weaponTf.localScale = tf.Scale;
    }

    private void SetWeaponGrabPoint(WeaponTransform point, WeaponTransform hint)
    {
        grabPosition.SetLocalPositionAndRotation(point.Position, Quaternion.Euler(point.Rotation));
        hintPosition.SetLocalPositionAndRotation(hint.Position, Quaternion.Euler(hint.Rotation));
    }

    #region AnimationEvent
    
    public void EquipWeapon(float weightTime)
    {
        var objTf = currentWeapon.transform;
        
        objTf.SetParent(handPos);
        objTf.SetLocalPositionAndRotation(_weaponData.HandTf.Position,
            Quaternion.Euler(_weaponData.HandTf.Rotation));
        objTf.localScale = _weaponData.HandTf.Scale;

        StartSetWeightRoutine(weightTime);
    }

    public void StartReloadRoutine()
    {
        _reloadRoutine = ReloadRoutine();
        StartCoroutine(_reloadRoutine);
    }

    public void SetWeight(float weight)
    {
        StartSetWeightRoutine(0.01f, weight);
    }

    public void UnEquipWeapon()
    {
        var objTf = currentWeapon.transform;
        
        objTf.SetParent(backPos);
        objTf.SetLocalPositionAndRotation(_weaponData.BackTf.Position,
            Quaternion.Euler(_weaponData.BackTf.Rotation));
        objTf.localScale = _weaponData.BackTf.Scale;
    }

    private void StartSetWeightRoutine(float weightTime, float weight = 1.0f)
    {
        _setWeightRoutine = SetWeightRoutine(weightTime, weight);
        StartCoroutine(_setWeightRoutine);
    }

    private IEnumerator SetWeightRoutine(float weightTime, float weight)
    {
        if (weightTime > 0.1)
        {
            grabPosition.SetLocalPositionAndRotation(_weaponData.IdleTargetPoint.Position,
                Quaternion.Euler(_weaponData.IdleTargetPoint.Rotation));

            hintPosition.SetLocalPositionAndRotation(_weaponData.IdleHintPoint.Position,
                Quaternion.Euler(_weaponData.IdleHintPoint.Rotation));
        }
        
        yield return new WaitForSeconds(weightTime);
        
        leftHandGrab.weight = weight;
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(0.05f);
        
        leftHandGrab.weight = 0;
        
        yield return new WaitForSeconds(2.5f);
        
        leftHandGrab.weight = 1;
        IsReloading = false;
        currentWeapon.Reload();
    }

    #endregion
}
