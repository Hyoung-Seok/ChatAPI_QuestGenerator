using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeadState : ZombieBaseState
{
    private const float DisableTime = 5.0f;
    private float _curTime = 0.0f;
    
    public ZombieDeadState(ZombieController controller) : base(controller) {}
    
    public override void Enter()
    {
        Controller.NavMeshAgent.ResetPath();
        Controller.Animator.SetBool(Controller.DeadKey, true);
        _curTime = 0;
    }

    public override void OnUpdate()
    {
        _curTime += Time.deltaTime;
        if (_curTime <= DisableTime)
        {
            return;
        }
        
        Controller.ChangeMainState(Controller.ZombieIdleState);
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnLateUpdate()
    {

    }

    public override void Exit()
    {
        Controller.Animator.SetBool(Controller.DeadKey, false);
        Controller.ResetEnemy();
    }
}
