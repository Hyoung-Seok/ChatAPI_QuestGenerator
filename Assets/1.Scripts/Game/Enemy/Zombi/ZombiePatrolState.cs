using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolState : ZombieBaseState
{
    private readonly int _walkKey = Animator.StringToHash("IsWalk");
    
    private readonly float _patrolRange = 0.0f;
    private Vector3 _targetPos = Vector3.zero;
    
    public ZombiePatrolState(ZombieController controller, ZombieStatus status) : base(controller)
    {
        _patrolRange = status.PatrolRange;
        Controller.ChangeSpeed(false);
    }
    
    public override void Enter()
    {
        for (var i = 0; i < 10; ++i)
        {
            _targetPos = GetRandomPatrolPos();
            
            if(_targetPos != Vector3.zero) break;
        }

        Controller.NavMeshAgent.SetDestination(_targetPos);
        Controller.Animator.SetBool(_walkKey, true);
        
        Controller.Tmp.text = "CurrentState : Patrol";
    }

    public override void OnUpdate()
    {
        if (Controller.DetectTarget() == true)
        {
            Controller.ChangeMainState(Controller.ZombieChaseState);
        }
        
        if (Controller.NavMeshAgent.remainingDistance <= 1.0f)
        {
            Controller.ChangeMainState(Controller.ZombieIdleState);
        }
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnLateUpdate()
    {

    }

    public override void Exit()
    {
        Controller.Animator.SetBool(_walkKey, false);
    }

    private Vector3 GetRandomPatrolPos()
    {
        var randDir = Random.insideUnitSphere * _patrolRange;
        randDir += Controller.GameObject.transform.position;

        return NavMesh.SamplePosition(randDir, out var hit, _patrolRange, NavMesh.AllAreas)
            ? hit.position
            : Vector3.zero;
    }
}
