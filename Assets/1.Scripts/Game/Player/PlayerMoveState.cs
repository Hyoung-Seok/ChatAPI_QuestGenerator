using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private PlayerMoveData _moveData;
    
    private float _horizontal;
    private float _vertical;
    private float _curSpeed = 0.0f;
    
    private Vector3 _camForward = Vector3.zero;
    private Vector3 _camRight = Vector3.zero;
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 _lookDir = Vector3.zero;
    private Vector3 _prevDir = Vector3.zero;

    public PlayerMoveState(PlayerController controller, PlayerMoveData data) : base(controller)
    {
        _moveData = data;
    }
    
    public override void Enter()
    {
        _prevDir = Controller.PlayerTransform.forward;
    }

    public override void OnUpdate()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        
        InitCameraDir();
        
        if (_horizontal != 0 || _vertical != 0)
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, _moveData.MoveSpeed, _moveData.Acceleration * Time.deltaTime);
            
            _moveDir = (_camForward * _vertical) + (_camRight * _horizontal);
            _moveDir.Normalize();
            _lookDir = _moveDir;
        }
        else
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, 0, _moveData.Deceleration * Time.deltaTime);
            
            _moveDir = _prevDir;
            _moveDir.Normalize();
            _lookDir = _moveDir;
        }
        
        _lookDir = _curSpeed != 0.0f && _lookDir != Vector3.zero ? _lookDir : Controller.PlayerTransform.forward;
        
        var targetRot = Quaternion.LookRotation(_lookDir, Vector3.up);
        Controller.PlayerTransform.rotation = Quaternion.Lerp(Controller.PlayerTransform.rotation,
            targetRot, _moveData.RotationSpeed * Time.deltaTime);
        
        _prevDir = _lookDir;
    }

    public override void OnFixedUpdate()
    {
        Controller.Rigidbody.velocity = _moveDir * _curSpeed;
        _prevDir = _lookDir;
    }

    public override void OnLateUpdate()
    {
        
    }

    public override void Exit()
    {
        
    }

    private void InitCameraDir()
    {
        _camForward = Controller.CameraDir.forward;
        _camForward.y = 0;
        _camForward.Normalize();
        
        _camRight = Controller.CameraDir.right;
        _camRight.y = 0;
        _camRight.Normalize();
    }
}
