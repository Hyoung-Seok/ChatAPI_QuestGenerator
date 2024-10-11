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

    private bool _isSetIdleAnimation = false;

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
        UpdatePlayerSpeed();
        
        if (_horizontal != 0 || _vertical != 0)
        {
            _moveDir = (_camForward * _vertical) + (_camRight * _horizontal);
            _moveDir.Normalize();
            _lookDir = _moveDir;
        }
        else
        {
            _moveDir = _prevDir;
            _moveDir.Normalize();
            _lookDir = _moveDir;
        }

        if (Controller.CurCameraState == ECameraState.IDLE)
        {
            _lookDir = _curSpeed != 0.0f && _lookDir != Vector3.zero ? _lookDir : Controller.PlayerTransform.forward;
        }
        else
        {
            _lookDir = _camForward;
        }
        
        var targetRot = Quaternion.LookRotation(_lookDir, Vector3.up);
        Controller.PlayerTransform.rotation = Quaternion.Slerp(Controller.PlayerTransform.rotation,
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

    public void OnValueUpdate(PlayerMoveData data)
    {
        _moveData = data;
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

    private void UpdatePlayerSpeed()
    {
        if (_horizontal != 0 || _vertical != 0)
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, _moveData.MoveSpeed, _moveData.Acceleration * Time.deltaTime);
            _isSetIdleAnimation = false;
        }
        else
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, 0, _moveData.Deceleration * Time.deltaTime);
        }

        if (_isSetIdleAnimation == false && _curSpeed <= 0.1f) { SetIdleAnimation(); }
        
        Controller.Animator.SetFloat(Controller.SpeedKey, _curSpeed);
    }

    private void SetIdleAnimation()
    {
        Controller.ChangeIdleAnimation();
        _isSetIdleAnimation = true;
    }
}
