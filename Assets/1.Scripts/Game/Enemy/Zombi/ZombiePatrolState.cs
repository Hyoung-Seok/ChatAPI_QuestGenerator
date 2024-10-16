using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolState : ZombieBaseState
{
    private float _patrolRange = 0.0f;
    private Vector3 _targetPos = Vector3.zero;
    
    public ZombiePatrolState(ZombieController controller, ZombieStatus status) : base(controller)
    {
        _patrolRange = status.PatrolRange;
    }
    
    public override void Enter()
    {
        for (var i = 0; i < 10; ++i)
        {
            _targetPos = GetRandomPatrolPos();
            
            if(_targetPos != Vector3.zero) break;
        }

        Controller.NavMeshAgent.SetDestination(_targetPos);
    }

    public override void OnUpdate()
    {

    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnLateUpdate()
    {

    }

    public override void Exit()
    {

    }

    private Vector3 GetRandomPatrolPos()
    {
        var randDir = Random.insideUnitSphere * _patrolRange;
        randDir += Controller.Tf.position;

        return NavMesh.SamplePosition(randDir, out var hit, _patrolRange, NavMesh.AllAreas)
            ? hit.position
            : Vector3.zero;
    }
}