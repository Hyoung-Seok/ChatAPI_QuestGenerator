using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackState : ZombieBaseState
{
    private readonly float _attackRange;
    private readonly float _attackDelay;
    private readonly float _detectedRange;
    private readonly Transform _tf;
    private readonly int _attackTrigger = Animator.StringToHash("IsAttack");
    private readonly int _attackAni = Animator.StringToHash("AttackAni");

    private float _curTime;
    
    public ZombieAttackState(ZombieController controller, ZombieStatus status) : base(controller)
    {
        _attackRange = status.AttackRange;
        _attackDelay = status.AttackDelay;
        _detectedRange = status.DetectRange;
        
        _tf = Controller.GameObject.transform;
    }
    
    public override void Enter()
    {
        _curTime = 0;
        Controller.NavMeshAgent.ResetPath();
        Controller.NavMeshAgent.autoBraking = false;
        
        Controller.Animator.SetBool(Controller.RunKey, false);
        Controller.Animator.SetInteger(_attackAni, Random.Range(0,2));
        Controller.Animator.SetTrigger(_attackTrigger);
        
        Controller.Tmp.text = "CurrentState : Attack";
    }

    public override void OnUpdate()
    {
        _curTime += Time.deltaTime;
        
        if (_curTime <= _attackDelay)
        {
            return;
        }

        var distance = Vector3.Distance(Controller.TargetTf.position, _tf.position);

        if (distance <= _attackRange)
        {
            _curTime = 0;
            Controller.Animator.SetInteger(_attackAni, Random.Range(0,2));
            Controller.Animator.SetTrigger(_attackTrigger);
            
            return;
        }

        if (distance > _attackRange && distance <= _detectedRange)
        {
            Controller.ChangeMainState(Controller.ZombieChaseState);
            return;
        }
        
        Controller.ChangeMainState(Controller.ZombieReturnState);
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnLateUpdate()
    {

    }

    public override void Exit()
    {
        _curTime = 0;
        Controller.Animator.ResetTrigger(_attackTrigger);
        Controller.NavMeshAgent.autoBraking = true;
    }
}
