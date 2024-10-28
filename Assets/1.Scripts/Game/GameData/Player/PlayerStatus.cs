using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Idle Move")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float runSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float acceleration = 20.0f;
    [SerializeField] private float deceleration = 40.0f;

    [Header("Equipped Move")] 
    [SerializeField] private float equippedMoveSpeed = 10.0f;
    [SerializeField] private float equippedRunSpeed = 10.0f;
    [SerializeField] private float aimMoveSpeed = 10.0f;

    [Header("Character Status")] 
    [SerializeField] private float moveInputSmooth = 2;
    [SerializeField] private float maxHP = 100.0f;
    [SerializeField] private int level = 1;

    public Action<PlayerStatus> OnValueChangeAction;

    #region MoveProperty
    
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float RotationSpeed => rotationSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float EquippedMoveSpeed => equippedMoveSpeed;
    public float EquippedRunSpeed => equippedRunSpeed;
    public float AimMoveSpeed => aimMoveSpeed;
    public float MoveInputSmooth => moveInputSmooth;
    
    #endregion

    #region Status

    public float MaxHp => maxHP;
    public int Level => level;

    #endregion

    private void OnValidate()
    {
        OnValueChangeAction?.Invoke(this);
    }
}
