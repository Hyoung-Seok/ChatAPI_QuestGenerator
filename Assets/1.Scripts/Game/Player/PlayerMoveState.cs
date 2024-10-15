using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private PlayerStatus _status;
    
    private float _horizontal;
    private float _vertical;
    private float _maxMoveSpeed;
    
    private float _curSpeed = 0.0f;
    
    private Vector3 _camForward = Vector3.zero;
    private Vector3 _camRight = Vector3.zero;
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 _lookDir = Vector3.zero;
    private Vector3 _prevDir = Vector3.zero;

    private bool _isSetIdleAnimation = false;

    public PlayerMoveState(PlayerController controller, PlayerStatus data) : base(controller)
    {
        _status = data;
        _maxMoveSpeed = _status.MoveSpeed;
    }

    public void ChangeMoveSpeed(EPlayerInputState state, bool isEquipped)
    {
        _maxMoveSpeed = state switch
        {
            EPlayerInputState.AIM => _status.AimMoveSpeed,
            
            EPlayerInputState.IDLE when (isEquipped == false) => _status.MoveSpeed,
            EPlayerInputState.IDLE => _status.EquippedMoveSpeed,
            
            EPlayerInputState.WALK when (isEquipped == false) => _status.MoveSpeed,
            EPlayerInputState.WALK => _status.EquippedMoveSpeed,
            
            EPlayerInputState.RUN when (isEquipped == false) => _status.RunSpeed,
            EPlayerInputState.RUN => _status.EquippedRunSpeed,
            
            EPlayerInputState.EQUIPPED => _status.EquippedMoveSpeed,
            
            _ => _status.MoveSpeed
        };
    }
    
    public override void Enter()
    {
        _prevDir = Controller.Transform.forward;
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

        if (Controller.CurInputState == EPlayerInputState.AIM)
        {
            _lookDir = _camForward;
        }
        else
        {
            _lookDir = _curSpeed != 0.0f && _lookDir != Vector3.zero ? _lookDir : Controller.Transform.forward;
        }
        
        var targetRot = Quaternion.LookRotation(_lookDir, Vector3.up);
        Controller.Transform.rotation = Quaternion.Slerp(Controller.Transform.rotation,
            targetRot, _status.RotationSpeed * Time.deltaTime);
        
        _prevDir = _lookDir;
    }

    public override void OnFixedUpdate()
    {
        Controller.PlayerRig.velocity = _moveDir * _curSpeed;
        _prevDir = _lookDir;
    }

    public override void OnLateUpdate()
    {
        
    }

    public override void Exit()
    {
        
    }

    public void OnValueUpdate(PlayerStatus data)
    {
        _status = data;
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
            _curSpeed = Mathf.MoveTowards(_curSpeed, _maxMoveSpeed, _status.Acceleration * Time.deltaTime);
            _isSetIdleAnimation = false;
        }
        else
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, 0, _status.Deceleration * Time.deltaTime);
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
