using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseController : UnitStateController
{
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Rigidbody Rig { get; private set; }
    public Animator Animator { get; private set; }
    public GameObject GameObject { get; private set; }
    
    // Detect member
    public Transform TargetTf { get; protected set; }
    public float DetectDistance { get; protected set; }
    public float DetectAngle { get; protected set; }
    
    private Vector3 _forward = Vector3.zero;
    private Vector3 _position = Vector3.zero;
    private Vector3 _targetDir = Vector3.zero;
    private float _targetDot = 0;
    private readonly LayerMask _layerMask;

    public virtual void ResetEnemy(Vector3 pos) {}
    
    protected EnemyBaseController(GameObject obj)
    {
        NavMeshAgent = obj.GetComponent<NavMeshAgent>();
        Rig = obj.GetComponent<Rigidbody>();
        Animator = obj.GetComponent<Animator>();
        GameObject = obj;
        TargetTf = GameManager.Instance.PlayerComponent.PlayerTransform;
        
        _layerMask = LayerMask.GetMask("Enemy");
    }

    public bool DetectTarget()
    {
        if(Vector3.Distance(TargetTf.position, GameObject.transform.position) > DetectDistance)
        {
            return false;
        }
        
        _position = GameObject.transform.position;
        _forward = GameObject.transform.forward;
        
        if(GameManager.Instance.IsDebug) DrawView();

        _targetDir = (TargetTf.position - _position).normalized;
        _targetDot = Vector3.Dot(_forward, _targetDir);

        if (_targetDot > Mathf.Cos(DetectAngle * Mathf.Deg2Rad) == false)
        {
            Debug.DrawLine(_position, TargetTf.position, Color.red);
            return false;
        }

        if (Physics.SphereCast(_position, 0.5f,_targetDir, out var hit, DetectDistance, ~_layerMask) == false)
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
        var left = Quaternion.AngleAxis(-DetectAngle, Vector3.up) * _forward;
        var right = Quaternion.AngleAxis(DetectAngle, Vector3.up) * _forward;
        
        Debug.DrawLine(_position, _position + left * DetectDistance, Color.white);
        Debug.DrawLine(_position, _position + right * DetectDistance, Color.white);
    }
}
