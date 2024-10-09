using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    [Header("Transform - Aim")]
    [SerializeField] private Vector3 onAimPosition;
    [SerializeField] private Vector3 onAimRotation;
    [SerializeField] private Vector3 onAimScale;

    [Header("Transform - HandGrab")] 
    [SerializeField] private List<Vector3> targetPoint;
    [SerializeField] private List<Vector3> hintPoint;

    [Header("Object")] 
    [SerializeField] private GameObject weaponModel;

    [Header("State")] 
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private int magazine;

    public Vector3 OnBackPosition => onBackPosition;
    public Vector3 OnBackRotation => onBackRotation;
    public Vector3 OnBackScale => onBackScale;
    
    public Vector3 OnAimPosition => onAimPosition;
    public Vector3 OnAimRotation => onAimRotation;
    public Vector3 OnAimScale => onAimScale;

    public Vector3 OnHandPosition => onHandPosition;
    public Vector3 OnHandRotation => onHandRotation;
    public Vector3 OnHandScale => onHandScale;
    
    public List<Vector3> TargetPointOnIdle => targetPoint;
    public List<Vector3> HintPointOnIdle => hintPoint;

    public GameObject WeaponModel => weaponModel;

    public float Damage => damage;
    public float FireRate => fireRate;
    public int Magazine => magazine;
}
