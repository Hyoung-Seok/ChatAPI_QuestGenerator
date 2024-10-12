using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private PlayerMoveData _moveData;
    
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
    private Transform _playerTf;
    private Transform _cameraTf;
    private Animator _playerAnimator;
    private Rigidbody _rig;

    public PlayerMoveState(PlayerController controller, PlayerMoveData data) : base(controller)
    {
        _moveData = data;
        _maxMoveSpeed = _moveData.MoveSpeed;
    }

    public void ChangeMoveSpeed(EPlayerInputState state, bool isEquipped)
    {
        _maxMoveSpeed = state switch
        {
            EPlayerInputState.AIM => _moveData.AimMoveSpeed,
            
            EPlayerInputState.IDLE when (isEquipped == false) => _moveData.MoveSpeed,
            EPlayerInputState.IDLE => _moveData.EquippedMoveSpeed,
            
            EPlayerInputState.WALK when (isEquipped == false) => _moveData.MoveSpeed,
            EPlayerInputState.WALK => _moveData.EquippedMoveSpeed,
            
            EPlayerInputState.RUN when (isEquipped == false) => _moveData.RunSpeed,
            EPlayerInputState.RUN => _moveData.EquippedRunSpeed,
            
            EPlayerInputState.EQUIPPED => _moveData.EquippedMoveSpeed,
            
            _ => _moveData.MoveSpeed
        };
    }
    
    public override void Enter()
    {
        _playerTf = GameManager.Instance.PlayerComponent.PlayerTransform;
        _rig = GameManager.Instance.PlayerComponent.Rig;
        _cameraTf = GameManager.Instance.PlayerComponent.CameraDir;
        _playerAnimator = GameManager.Instance.PlayerComponent.Animator;
        
        _prevDir = _playerTf.forward;
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
            _lookDir = _curSpeed != 0.0f && _lookDir != Vector3.zero ? _lookDir : _playerTf.forward;
        }
        
        var targetRot = Quaternion.LookRotation(_lookDir, Vector3.up);
        _playerTf.rotation = Quaternion.Slerp(_playerTf.rotation,
            targetRot, _moveData.RotationSpeed * Time.deltaTime);
        
        _prevDir = _lookDir;
    }

    public override void OnFixedUpdate()
    {
        _rig.velocity = _moveDir * _curSpeed;
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
        _camForward = _cameraTf.forward;
        _camForward.y = 0;
        _camForward.Normalize();
        
        _camRight = _cameraTf.right;
        _camRight.y = 0;
        _camRight.Normalize();
    }

    private void UpdatePlayerSpeed()
    {
        if (_horizontal != 0 || _vertical != 0)
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, _maxMoveSpeed, _moveData.Acceleration * Time.deltaTime);
            _isSetIdleAnimation = false;
        }
        else
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, 0, _moveData.Deceleration * Time.deltaTime);
        }

        if (_isSetIdleAnimation == false && _curSpeed <= 0.1f) { SetIdleAnimation(); }
        
        _playerAnimator.SetFloat(Controller.SpeedKey, _curSpeed);
    }

    private void SetIdleAnimation()
    {
        Controller.ChangeIdleAnimation();
        _isSetIdleAnimation = true;
    }
}
