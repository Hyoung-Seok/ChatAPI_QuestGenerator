using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseController : UnitStateController
{
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Rigidbody Rig { get; private set; }
    public Transform Tf { get; private set; }
    public Animator Animator { get; private set; }

    public EnemyBaseController(EnemyComponent component)
    {
        NavMeshAgent = component.NavMeshAgent;
        Rig = component.Rig;
        Tf = component.Tf;
        Animator = component.Animator;
    }
}
