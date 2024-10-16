using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    private Transform _targetTf;
    private float _returnDistance;
    private float _screamChance;
    private float _attackRange;
    
    public ZombieChaseState(ZombieController controller, ZombieStatus status) : base(controller)
    {
        _targetTf = Controller.TargetTf;
        
        _returnDistance = status.ReturnDistance;
        _screamChance = status.ScreamChance;
        _attackRange = status.AttackRange;
    }
    
    public override void Enter()
    {
        Controller.NavMeshAgent.stoppingDistance = 0.8f;
        Controller.NavMeshAgent.autoBraking = false;
        
        Controller.Animator.SetBool(Controller.RunKey, true);
        Controller.ChangeSpeed(true);
    }

    public override void OnUpdate()
    {
        Controller.NavMeshAgent.SetDestination(_targetTf.position);
        var curDistance = Vector3.Distance(_targetTf.position, Controller.Tf.position);

        if (curDistance <= _attackRange)
        {
            Debug.Log("Attack!!");
        }

        if (curDistance >= _returnDistance)
        {
            Debug.Log("Return");
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
        Controller.Animator.SetBool(Controller.RunKey, false);
        Controller.NavMeshAgent.autoBraking = true;
    }
}
