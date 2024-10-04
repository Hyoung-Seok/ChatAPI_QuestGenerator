using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    [Header("Value")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float acceleration = 20.0f;
    [SerializeField] private float deceleration = 40.0f;

    private float _horizontal;
    private float _vertical;
    private float _curSpeed = 0.0f;
    private bool _isMove = false;
    private Vector3 _camForward = Vector3.zero;
    private Vector3 _camRight = Vector3.zero;
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 _lookDir = Vector3.zero;
    private Vector3 _prevDir = Vector3.zero;
    
    public override void Enter()
    {
        _prevDir = transform.forward;
    }

    public override void OnUpdate()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        if (_horizontal != 0 || _vertical != 0)
        {
            _isMove = true;
            
        }
        else
        {
            _isMove = false;
        }
        
        UpdateMoveSpeed();
    }

    public override void OnFixedUpdate()
    {
        InitCameraDir();
        
        if (_isMove == true)
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
        
        _lookDir = _curSpeed != 0.0f && _lookDir != Vector3.zero ? _lookDir : transform.forward;

        var targetRot = Quaternion.LookRotation(_lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
            rotationSpeed * Time.fixedDeltaTime);
        
        Controller.PlayerRig.velocity = _moveDir * _curSpeed;
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

    private void UpdateMoveSpeed()
    {
        if (_isMove == true)
        {
            _curSpeed = Mathf.MoveTowards(_curSpeed, moveSpeed, acceleration * Time.deltaTime);
            return;
        }
        
        _curSpeed = Mathf.MoveTowards(_curSpeed, 0, deceleration * Time.deltaTime);
    }
}
