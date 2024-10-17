using UnityEngine;

public class ZombieReturnState : ZombieBaseState
{
    private readonly int _walkKey = Animator.StringToHash("IsWalk");
    
    public ZombieReturnState(ZombieController controller) : base(controller) { }
    
    public override void Enter()
    {
        Controller.ChangeSpeed(false);
        Controller.Animator.SetBool(_walkKey, true);

        Controller.NavMeshAgent.SetDestination(Controller.OriginPosition);
    }

    public override void OnUpdate()
    {
        if (Controller.DetectTarget() == true)
        {
            Controller.ChangeMainState(Controller.ZombieChaseState);
        }

        if (Controller.NavMeshAgent.remainingDistance <= 1.0f)
        {
            Controller.ChangeMainState(Controller.ZombieIdleState);
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
        Controller.Animator.SetBool(_walkKey, false);
        Controller.NavMeshAgent.ResetPath();
    }
}
