using TMPro;
using UnityEngine;

public class ZombieController : EnemyBaseController
{
    // state
    public ZombieIdleState ZombieIdleState { get; private set; }
    public ZombiePatrolState ZombiePatrolState { get; private set; }
    public ZombieChaseState ZombieChaseState { get; private set; }
    public ZombieAttackState ZombieAttackState { get; private set; }
    public ZombieReturnState ZombieReturnState { get; private set; }

    public TextMeshPro Tmp;

    // HashKey
    public readonly int RunKey = Animator.StringToHash("IsRun");
    
    public ZombieController(GameObject obj, ZombieStatus status) : base(obj, status)
    {
        Tmp = obj.transform.GetChild(2).GetComponent<TextMeshPro>();
        var hitEvent = obj.GetComponent<OnPhysicsEvent>();

        hitEvent.OnHitFunc -= HitEvent;
        hitEvent.OnHitFunc += HitEvent;
        
        ChangeSpeed(false);

        ZombieIdleState = new ZombieIdleState(this, status);
        ZombiePatrolState = new ZombiePatrolState(this, status);
        ZombieChaseState = new ZombieChaseState(this, status);
        ZombieAttackState = new ZombieAttackState(this, status);
        ZombieReturnState = new ZombieReturnState(this);
        
        ChangeMainState(ZombieIdleState);
    }

    public override void HitEvent(float dmg)
    {
        CurrentHp -= dmg;

        if (CurrentHp <= 0)
        {
            
        }
    }

    public override void ResetEnemy()
    {
        NavMeshAgent.stoppingDistance = OriginStopDistance;
        
        Animator.SetBool(RunKey, false);
        ChangeMainState(ZombieIdleState);
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
