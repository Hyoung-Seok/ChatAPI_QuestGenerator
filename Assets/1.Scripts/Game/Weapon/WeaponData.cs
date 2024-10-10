using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Object/WeaponData", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{
    [Header("Transform - Back")] 
    [SerializeField] private WeaponTransform backTf;

    [Header("Transform - Hand")] 
    [SerializeField] private WeaponTransform handTf;

    [Header("Transform - Aim")] 
    [SerializeField] private WeaponTransform aimTf;

    [Header("Transform - HandGrab")] 
    [SerializeField] private WeaponTransform idleTargetPoint;
    [SerializeField] private WeaponTransform aimTargetPoint;
    [SerializeField] private WeaponTransform idleHintPoint;
    [SerializeField] private WeaponTransform aimHintPoint;

    [Header("Object")] 
    [SerializeField] private GameObject cartridge;

    [Header("State")] 
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private int magazine;
    [SerializeField, Range(0, 1)] private float recoilX;
    [SerializeField, Range(0, 1)] private float recoilY;
    
    public WeaponTransform BackTf => backTf;
    public WeaponTransform HandTf => handTf;
    public WeaponTransform AimTf => aimTf;

    public WeaponTransform IdleTargetPoint => idleTargetPoint;
    public WeaponTransform AimTargetPoint => aimTargetPoint;
    public WeaponTransform IdleHintPoint => idleHintPoint;
    public WeaponTransform AimHintPoint => aimHintPoint;
    
    public GameObject Cartridge => cartridge;

    public float Damage => damage;
    public float FireRate => fireRate;
    public int Magazine => magazine;
    public float RecoilX => recoilX;
    public float RecoilY => recoilY;
}
