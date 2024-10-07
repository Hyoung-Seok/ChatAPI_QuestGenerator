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

    [Header("Stat")] 
    [SerializeField] private float MaxHP = 100.0f;

    [Header("Animation Hash")]
    public readonly int SpeedKey = Animator.StringToHash("Speed");
    private readonly int _idleAnimation = Animator.StringToHash("IdleMotions");
    private readonly int _playerHpKey = Animator.StringToHash("PlayerHP");
    
    [Header("State")]
    private PlayerBaseState _moveState;

    private float _currentHp;
    
    public void Init(PlayerMoveData data)
    {
        _moveState = new PlayerMoveState(this, data);
        
        _currentHp = MaxHP;
        Animator.SetFloat(_playerHpKey, _currentHp);
        
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
}
