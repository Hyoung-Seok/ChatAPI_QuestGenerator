using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ZombieChaseState : ZombieBaseState
{
    private readonly Transform _targetTf;
    private readonly float _returnDistance;
    private readonly int _screamChance;
    private readonly float _attackRange;
    private readonly int _screamKey = Animator.StringToHash("IsScream");
    
    private float _curTime = 0;
    private float _waitingTime = 0; 
    
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

        if (Controller.TryExecuteAction(_screamChance) == true)
        {
            _waitingTime = 2.0f;
            Controller.Animator.SetTrigger(_screamKey);
            ScreamDelay();
        }
        else
        {
            _waitingTime = 0;           
            Controller.Animator.SetBool(Controller.RunKey, true);
            Controller.ChangeSpeed(true);
        }
    }

    public override void OnUpdate()
    {
        _curTime += Time.deltaTime;

        if (_curTime <= _waitingTime)
        {
            return;
        }
        
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
        _curTime = 0;
    }

    private async void ScreamDelay()
    {
        await Task.Delay(2000);
        
        Controller.Animator.SetBool(Controller.RunKey, true);
        Controller.ChangeSpeed(true);
    }
}
