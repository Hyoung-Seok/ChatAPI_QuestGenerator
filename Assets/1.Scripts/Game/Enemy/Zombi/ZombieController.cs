using UnityEngine;

public class ZombieController : EnemyBaseController
{
    // state
    public ZombieIdleState ZombieIdleState { get; private set; }
    public ZombiePatrolState ZombiePatrolState { get; private set; }
    public ZombieChaseState ZombieChaseState { get; private set; }
    public ZombieAttackState ZombieAttackState { get; private set; }
    public ZombieReturnState ZombieReturnState { get; private set; }

    // status
    private readonly float _moveSpeed = 0.0f;
    private readonly float _runSpeed = 0.0f;

    // HashKey
    public readonly int RunKey = Animator.StringToHash("IsRun");
    
    // Property
    public float OriginStopDistance { get; private set; }
    public Vector3 OriginPosition { get; private set; }

    public ZombieController(EnemyComponent component, ZombieStatus status) : base(component)
    {
        //TODO : 추후 플레이어로 변경
        TargetTf = TestManager.Instance.Target;
        DetectDistance = status.DetectRange;
        DetectAngle = status.DetectAngle;
        
        _moveSpeed = status.MoveSpeed;
        _runSpeed = status.RunSpeed;
        NavMeshAgent.angularSpeed = status.RotationSpeed;

        OriginStopDistance = NavMeshAgent.stoppingDistance;
        OriginPosition = Tf.position;
        
        ChangeSpeed(false);

        ZombieIdleState = new ZombieIdleState(this, status);
        ZombiePatrolState = new ZombiePatrolState(this, status);
        ZombieChaseState = new ZombieChaseState(this, status);
        ZombieAttackState = new ZombieAttackState(this, status);
        ZombieReturnState = new ZombieReturnState(this);
        
        ChangeMainState(ZombieIdleState);
    }

    public void ChangeSpeed(bool isChase)
    {
        NavMeshAgent.speed = isChase ? _runSpeed : _moveSpeed;
    }

    public bool TryExecuteAction(int chance)
    {
        return Random.Range(1, 101) < chance;
    }
}
