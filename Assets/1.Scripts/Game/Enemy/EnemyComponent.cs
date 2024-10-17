using UnityEngine;
using UnityEngine.AI;

public class EnemyComponent
{
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Rigidbody Rig { get; private set; }
    public Transform Tf { get; private set; }
    public Animator Animator { get; private set; }

    public EnemyComponent(NavMeshAgent nav, Rigidbody rig, Transform tf, Animator ani)
    {
        NavMeshAgent = nav;
        Rig = rig;
        Tf = tf;
        Animator = ani;
    }
}
