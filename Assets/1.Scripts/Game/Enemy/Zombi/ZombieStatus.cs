using UnityEngine;

[CreateAssetMenu(fileName = "ZombieStatus", menuName = "Scriptable Object/ZombieStatus", order = int.MaxValue)]
public class ZombieStatus : ScriptableObject
{
    [Header("Status")] 
    [SerializeField] private float maxHp;
    [SerializeField] private float defense;
    [SerializeField] private float detectRange;
    [SerializeField] private float detectAngle;
    [SerializeField] private float returnDistance;
    [SerializeField] private float fireHearingRange;
    [SerializeField] private float footStepHearingRange;

    [Header("State - idle")] 
    [SerializeField] private float idleStateMinTime;
    [SerializeField] private float idleStateMaxTime;

    [Header("State - Patrol")] 
    [SerializeField] private float patrolRange;
    
    [Header("State - Move")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("State - Attack")] 
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDelay;
    [SerializeField] private float damage;

    [Header("Chance")] 
    [SerializeField, Range(1,100)] private int agonizingChance;
    [SerializeField, Range(1,100)] private int screamChance;

    public float MaxHp => maxHp;
    public float Defense => defense;
    public float Damage => damage;
    public float AttackRange => attackRange;
    public float AttackDelay => attackDelay;
    public float DetectRange => detectRange;
    public float DetectAngle => detectAngle;
    public float ReturnDistance => returnDistance;
    public float FireHearingRange => fireHearingRange;
    public float FootStepHearingRange => footStepHearingRange;
    public float IdleStateMinTime => idleStateMinTime;
    public float IdleStateMaxTime => idleStateMaxTime;
    public float PatrolRange => patrolRange;
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float RotationSpeed => rotationSpeed;
    public int AgonizingChance => agonizingChance;
    public int ScreamChance => screamChance;
}
