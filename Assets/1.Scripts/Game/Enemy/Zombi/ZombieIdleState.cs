using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    private readonly int _isAgonizing = Animator.StringToHash("IsAgonizing"); 
    
    private readonly float _minTime = 0;
    private readonly float _maxTime = 0;
    private readonly int _agonizingChance = 0;
    
    private float _curTime = 0;
    private float _transitionTime = 0;
    
    public ZombieIdleState(ZombieController controller, EnemyStatus status) : base(controller)
    {
        _minTime = status.IdleStateMinTime;
        _maxTime = status.IdleStateMaxTime;
        _agonizingChance = status.AgonizingChance;
    }
    
    public override void Enter()
    {
        _curTime = 0;
        _transitionTime = Random.Range(_minTime, _maxTime);

        if (Controller.TryExecuteAction(_agonizingChance) == true)
        {
            Controller.Animator.SetTrigger(_isAgonizing);
        }
        
        Controller.Tmp.text = "CurrentState : Idle";
    }

    public override void OnUpdate()
    {
        if (Controller.DetectTarget() == true)
        {
            Controller.ChangeMainState(Controller.ZombieChaseState);
        }
        
        _curTime += Time.deltaTime;
        
        if (_curTime >= _transitionTime)
        {
            Controller.ChangeMainState(Controller.ZombiePatrolState);
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

    }
}
