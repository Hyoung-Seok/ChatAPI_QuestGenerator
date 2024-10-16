using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : ZombieBaseState
{
    private readonly float _minTime = 0;
    private readonly float _maxTime = 0;
    
    private float _curTime = 0;
    private float _transitionTime = 0;
    
    public ZombieIdleState(ZombieController controller, ZombieStatus status) : base(controller)
    {
        _minTime = status.IdleStateMinTime;
        _maxTime = status.IdleStateMaxTime;
    }
    
    public override void Enter()
    {
        _curTime = 0;
        _transitionTime = Random.Range(_minTime, _maxTime);
        
        Debug.Log($"Time : {_transitionTime}");
    }

    public override void OnUpdate()
    {
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
