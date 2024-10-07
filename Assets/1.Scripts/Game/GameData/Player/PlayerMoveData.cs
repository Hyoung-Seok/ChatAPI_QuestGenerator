using UnityEngine;

public class PlayerMoveData : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float acceleration = 20.0f;
    [SerializeField] private float deceleration = 40.0f;
    
    public float MoveSpeed => moveSpeed;
    public float RotationSpeed => rotationSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
}
