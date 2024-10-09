using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class RigController : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private MultiAimConstraint bodyAimPoint;
    [SerializeField] private MultiAimConstraint armAimPoint;
    [SerializeField] private TwoBoneIKConstraint weaponGrabRig;
    [SerializeField] private AimTarget aimTarget;
    [SerializeField] private AnimationEvent animationEvent;

    [Header("GrabPoint")] 
    [SerializeField] private Transform grabPos;
    [SerializeField] private Transform hintPos;
    
    [Header("Rig Value")] 
    [SerializeField] private float maxWeight;
    private IEnumerator _equipRoutine;
    
    private void Start()
    {
        aimTarget.gameObject.SetActive(false);
    }

    private void StartEnableIKRoutine()
    {
        _equipRoutine = EnableTwoBonIKConstraintRoutine();
        StartCoroutine(_equipRoutine);
    }

    private void StarDisableIKRoutine()
    {
        _equipRoutine = DisableTwoBonIKConstraintRoutine();
        StartCoroutine(_equipRoutine);
    }
    
    public void StartAim()
    {
        aimTarget.gameObject.SetActive(true);
        
        bodyAimPoint.weight = maxWeight;
        armAimPoint.weight = maxWeight;
    }

    public void StopAim()
    {
        aimTarget.gameObject.SetActive(false);
        
        bodyAimPoint.weight = 0;
        armAimPoint.weight = 0;
    }
    
    private IEnumerator EnableTwoBonIKConstraintRoutine()
    {
        Debug.Log("Start");
        yield return new WaitForSeconds(0.01f);
        
        grabPos.localPosition = animationEvent.CurrentWeapon.TargetPointOnIdle[0];
        grabPos.localRotation = Quaternion.Euler(animationEvent.CurrentWeapon.TargetPointOnIdle[1]);

        hintPos.localPosition = animationEvent.CurrentWeapon.HintPointOnIdle[0];
        hintPos.localRotation = Quaternion.Euler(animationEvent.CurrentWeapon.HintPointOnIdle[1]);

        weaponGrabRig.weight = 1.0f;
    }

    private IEnumerator DisableTwoBonIKConstraintRoutine()
    {
        Debug.Log("Start");
        yield return new WaitForSeconds(0.01f);
        
        weaponGrabRig.weight = 0;
    }
}
