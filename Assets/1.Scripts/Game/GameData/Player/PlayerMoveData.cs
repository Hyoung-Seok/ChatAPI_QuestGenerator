using System;
using UnityEngine;

public class PlayerMoveData : MonoBehaviour
{
    [Header("IdleState")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float runSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float acceleration = 20.0f;
    [SerializeField] private float deceleration = 40.0f;

    [Header("Equipped State")] 
    [SerializeField] private float equippedMoveSpeed = 10.0f;
    [SerializeField] private float equippedRunSpeed = 10.0f;
    [SerializeField] private float aimMoveSpeed = 10.0f;

    public Action<PlayerMoveData> OnValueChangeAction;
    
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float RotationSpeed => rotationSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float EquippedMoveSpeed => equippedMoveSpeed;
    public float EquippedRunSpeed => equippedRunSpeed;

    public float AimMoveSpeed => aimMoveSpeed;

    private void OnValidate()
    {
        OnValueChangeAction?.Invoke(this);
    }
}
