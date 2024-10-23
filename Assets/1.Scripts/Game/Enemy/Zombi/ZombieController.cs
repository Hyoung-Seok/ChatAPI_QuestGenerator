using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieController : EnemyBaseController
{
    // state
    public ZombieIdleState ZombieIdleState { get; private set; }
    public ZombiePatrolState ZombiePatrolState { get; private set; }
    public ZombieChaseState ZombieChaseState { get; private set; }
    public ZombieAttackState ZombieAttackState { get; private set; }
    public ZombieReturnState ZombieReturnState { get; private set; }
    public ZombieDeadState ZombieDeadState { get; private set; }

    public TextMeshPro Tmp;

    // HashKey
    public readonly int RunKey = Animator.StringToHash("IsRun");
    public readonly int DeadKey = Animator.StringToHash("IsDead");
    
    public ZombieController(GameObject obj, ZombieStatus status, Func<ObjectPool> getEffect = null) : base(obj, status, getEffect)
    {
        Tmp = obj.transform.GetChild(3).GetComponent<TextMeshPro>();
        var physicsEvent = obj.GetComponent<OnPhysicsEvent>();
        
        physicsEvent.OnTakeDamage -= HitEvent;
        physicsEvent.OnTakeDamage += HitEvent;
        
        physicsEvent.OnHitTarget -= GameManager.Instance.Player.PlayerDamaged;
        physicsEvent.OnHitTarget += GameManager.Instance.Player.PlayerDamaged;
        
        ChangeSpeed(false);

        ZombieIdleState = new ZombieIdleState(this, status);
        ZombiePatrolState = new ZombiePatrolState(this, status);
        ZombieChaseState = new ZombieChaseState(this, status);
        ZombieAttackState = new ZombieAttackState(this, status);
        ZombieReturnState = new ZombieReturnState(this);
        ZombieDeadState = new ZombieDeadState(this);
        
        ChangeMainState(ZombieIdleState);
    }

    public override void HitEvent(float dmg, HitPoint hitPoint)
    {
        CurrentHp -= dmg;
        Debug.Log($"Cur HP : {CurrentHp}");

        var effect = GetHitEffect?.Invoke();

        if (effect is not null)
        {
            effect.transform.position = hitPoint.HitPosition;
            effect.transform.rotation = Quaternion.LookRotation(hitPoint.HitNormal);   
        }

        if (CurrentHp <= 0 && MainState != ZombieDeadState)
        {
            ChangeMainState(ZombieDeadState);
        }
    }

    public override void ResetEnemy()
    {
        // nav Mesh Reset
        NavMeshAgent.ResetPath();
        NavMeshAgent.autoBraking = true;
        NavMeshAgent.stoppingDistance = OriginStopDistance;
        
        // status Reset
        CurrentHp = MaxHp;
        
        // Animator Parameters Reset
        foreach (var param in Animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Int:
                    Animator.SetInteger(param.name, 0);
                    break;
                
                case AnimatorControllerParameterType.Bool:
                    Animator.SetBool(param.name, false);
                    break;
                
                case AnimatorControllerParameterType.Float:
                    Animator.SetFloat(param.name, 0);
                    break;
                
                case AnimatorControllerParameterType.Trigger:
                    Animator.ResetTrigger(param.name);
                    break;
                
                default:
                    return;
            }
        }
        
        ReturnAction?.Invoke(this);
    }

    public void ChangeSpeed(bool isChase)
    {
        NavMeshAgent.speed = isChase ? RunSpeed : MoveSpeed;
    }

    public bool TryExecuteAction(int chance)
    {
        return Random.Range(1, 101) < chance;
    }
}
