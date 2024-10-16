using UnityEngine;

public class ZombieController : EnemyBaseController
{
    // state
    public ZombieIdleState ZombieIdleState { get; private set; }
    public ZombiePatrolState ZombiePatrolState { get; private set; }

    private readonly float _moveSpeed = 0.0f;
    private readonly float _runSpeed = 0.0f;

    public ZombieController(EnemyComponent component, ZombieStatus status) : base(component)
    {
        _moveSpeed = status.MoveSpeed;
        _runSpeed = status.RunSpeed;
        NavMeshAgent.angularSpeed  = status.RotationSpeed;
        
        ChangeSpeed(false);

        ZombieIdleState = new ZombieIdleState(this, status);
        ZombiePatrolState = new ZombiePatrolState(this, status);
        
        ChangeMainState(ZombieIdleState);
    }

    public void ChangeSpeed(bool isChase)
    {
        NavMeshAgent.speed = isChase ? _runSpeed : _moveSpeed;
    }
}
