using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Object/WeaponData", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{
    [Header("Transform - Back")] 
    [SerializeField] private Vector3 onBackPosition;
    [SerializeField] private Vector3 onBackRotation;
    [SerializeField] private Vector3 onBackScale;
    
    [Header("Transform - Hand")]
    [SerializeField] private Vector3 onHandPosition;
    [SerializeField] private Vector3 onHandRotation;
    [SerializeField] private Vector3 onHandScale;

    [Header("Object")] 
    [SerializeField] private GameObject weaponModel;

    [Header("State")] 
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private int magazine;

    public Vector3 OnBackPosition => onBackPosition;
    public Vector3 OnBackRotation => onBackRotation;
    public Vector3 OnBackScale => onBackScale;

    public Vector3 OnHandPosition => onHandPosition;
    public Vector3 OnHandRotation => onHandRotation;
    public Vector3 OnHandScale => onHandScale;

    public GameObject WeaponModel => weaponModel;

    public float Damage => damage;
    public float FireRate => fireRate;
    public int Magazine => magazine;

    public Transform GetPistolGripPos()
    {
        return (weaponModel == null)
            ? null
            : weaponModel.transform.GetChild(weaponModel.transform.childCount - 2);
    }
    
    public Transform GetHandGardPos()
    {
        return (weaponModel == null)
            ? null
            : weaponModel.transform.GetChild(weaponModel.transform.childCount - 1);
    } 
}
