using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : ZombieBaseState
{
    private readonly Transform _targetTf;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly float _returnDistance;
    private readonly int _screamChance;
    private readonly float _attackRange;
    private readonly int _screamKey = Animator.StringToHash("IsScream");
    
    private float _curTime = 0;
    private float _waitingTime = 0;
    
    public ZombieChaseState(ZombieController controller, ZombieStatus status) : base(controller)
    {
        _targetTf = Controller.TargetTf;
        _navMeshAgent = Controller.NavMeshAgent;
        
        _returnDistance = status.ReturnDistance;
        _screamChance = status.ScreamChance;
        _attackRange = status.AttackRange;
    }
    
    public override void Enter()
    {
        _navMeshAgent.stoppingDistance = 0.8f;
        _navMeshAgent.autoBraking = false;
        _curTime = 0;

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
        
        _navMeshAgent.SetDestination(_targetTf.position);
        var curDistance = Vector3.Distance(_targetTf.position, Controller.GameObject.transform.position);

        if (curDistance <= _attackRange)
        {
            Controller.ChangeMainState(Controller.ZombieAttackState);
        }

        if (curDistance >= _returnDistance)
        {
            Controller.ChangeMainState(Controller.ZombieReturnState);
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
        _navMeshAgent.autoBraking = true;
        _navMeshAgent.stoppingDistance = Controller.OriginStopDistance;
        _curTime = 0;
    }

    private async void ScreamDelay()
    {
        await Task.Delay(2000);
        
        Controller.Animator.SetBool(Controller.RunKey, true);
        Controller.ChangeSpeed(true);
    }
}
