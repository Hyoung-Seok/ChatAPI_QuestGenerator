using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : UnitStateController
{
    [Header("Component")]
    public Rigidbody Rigidbody;
    public Transform PlayerTransform;
    public Transform CameraDir;
    public Animator Animator;
    [SerializeField] private RigController rigController;

    [Header("Stat")] 
    [SerializeField] private float MaxHP = 100.0f;

    [Header("Animation Hash")]
    public readonly int SpeedKey = Animator.StringToHash("Speed");
    private readonly int _idleAnimation = Animator.StringToHash("IdleMotions");
    private readonly int _playerHpKey = Animator.StringToHash("PlayerHP");
    private readonly int _quippedTriggerKey = Animator.StringToHash("Equipped");
    private readonly int _equippedKey = Animator.StringToHash("IsEquipped");
    private readonly int _aimKey = Animator.StringToHash("Aim");
    
    [Header("State")]
    private PlayerBaseState _moveState;
    
    [HideInInspector] public ECameraState CurCameraState;
    
    private float _currentHp;
    private bool _isEquipped = false;
    
    public void Init(PlayerMoveData data)
    {
        _moveState = new PlayerMoveState(this, data);
        
        _currentHp = MaxHP;
        Animator.SetFloat(_playerHpKey, _currentHp);

        _isEquipped = false;
        
        ChangeMainState(_moveState);
    }

    public void PlayerDamaged(float dmg)
    {
        _currentHp -= dmg;
        Animator.SetFloat(_playerHpKey, _currentHp);

        if (_currentHp <= 0)
        {
            // TODO : 사망처리
        }
    }

    public void ChangeIdleAnimation()
    {
        if (_currentHp < 30)
        {
            Animator.SetInteger(_idleAnimation, 0);
            return;
        }
        
        var num = Random.Range(1, 4);
        Animator.SetInteger(_idleAnimation, num);
    }

    public void EquippedWeapon()
    {
        Animator.SetTrigger(_quippedTriggerKey);

        if (_isEquipped == true)
        {
            Animator.SetBool(_equippedKey, _isEquipped);
            _isEquipped = false;
            
            return;
        }

        Animator.SetBool(_equippedKey, _isEquipped);
        _isEquipped = true;
    }

    public void ChangeCameraState(ECameraState state)
    {
        switch (state)
        {
            case ECameraState.AIM:
                rigController.StartAim();
                Animator.SetBool(_aimKey, true);
                break;
            
            case ECameraState.IDLE:
                rigController.StopAim();
                Animator.SetBool(_aimKey, false);
                break;
            
            default:
                return;
        }
        
        CurCameraState = state;
    }
}

public enum ECameraState
{
    IDLE,
    AIM
}
