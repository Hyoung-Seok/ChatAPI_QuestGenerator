using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseController : UnitStateController
{
    // Component
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Rigidbody Rig { get; private set; }
    public Animator Animator { get; private set; }
    public GameObject GameObject { get; private set; }
    
    // Origin
    public float OriginStopDistance { get; private set; }
    public Vector3 OriginPosition { get; private set; }
    
    // status
    protected readonly float MoveSpeed;
    protected readonly float RunSpeed;
    protected readonly float MaxHp;
    protected float CurrentHp;
        
    // Detect member
    private static readonly LayerMask LayerMask = LayerMask.GetMask("Enemy");
    public Transform TargetTf { get; set; }
    private readonly float _detectDistance;
    private readonly float _detectAngle;
    private Vector3 _forward = Vector3.zero;
    private Vector3 _position = Vector3.zero;
    private Vector3 _targetDir = Vector3.zero;
    private float _targetDot = 0;
    
    // Action
    public Action<EnemyBaseController> ReturnAction;
    protected readonly Func<ObjectPool> GetHitEffect;
    
    public virtual void ResetEnemy() {}
    public virtual void HitEvent(float dmg, HitPoint hitPoint) {}
    
    protected EnemyBaseController(GameObject obj, ZombieStatus status, Func<ObjectPool> hitEffectPool = null)
    {
        GetHitEffect = hitEffectPool;
        
        GameObject = obj;
        NavMeshAgent = GameObject.GetComponent<NavMeshAgent>();
        Rig = GameObject.GetComponent<Rigidbody>();
        Animator = GameObject.GetComponent<Animator>();
        TargetTf = GameManager.Instance.PlayerComponent.PlayerTransform;
        GameObject.GetComponent<OnPhysicsEvent>().SetDamage(status.Damage);
        
        // nav mesh set
        OriginStopDistance = NavMeshAgent.stoppingDistance;
        OriginPosition = obj.transform.position;
        
        // status set
        _detectDistance = status.DetectRange;
        _detectAngle = status.DetectAngle;
        MoveSpeed = status.MoveSpeed;
        RunSpeed = status.RunSpeed;
        NavMeshAgent.angularSpeed = status.RotationSpeed;
        CurrentHp = MaxHp = status.MaxHp;
    }
    
    public void SetPosition(Vector3 pos)
    {
        OriginPosition = pos;
        GameObject.transform.position = pos;
    }

    public void SetTargetTransform(Transform target)
    {
        TargetTf = target;
    }

    public bool DetectTarget()
    {
        if(Vector3.Distance(TargetTf.position, GameObject.transform.position) > _detectDistance)
        {
            return false;
        }
        
        _position = GameObject.transform.position;
        _forward = GameObject.transform.forward;
        
        if(GameManager.Instance.IsDebug) DrawView();

        _targetDir = (TargetTf.position - _position).normalized;
        _targetDot = Vector3.Dot(_forward, _targetDir);

        if (_targetDot > Mathf.Cos(_detectAngle * Mathf.Deg2Rad) == false)
        {
            Debug.DrawLine(_position, TargetTf.position, Color.red);
            return false;
        }

        if (Physics.SphereCast(_position, 0.5f,_targetDir, out var hit, _detectDistance, ~LayerMask) == false)
        {
            return false;
        }
        
        if (hit.collider.CompareTag("Player") == false)
        {
            Debug.DrawLine(_position, TargetTf.position, Color.red);
            return false;
        }
            
        Debug.DrawLine(_position, TargetTf.position, Color.blue);
        return true;
    }

    private void DrawView()
    {
        var left = Quaternion.AngleAxis(-_detectAngle, Vector3.up) * _forward;
        var right = Quaternion.AngleAxis(_detectAngle, Vector3.up) * _forward;
        
        Debug.DrawLine(_position, _position + left * _detectDistance, Color.white);
        Debug.DrawLine(_position, _position + right * _detectDistance, Color.white);
    }
}
