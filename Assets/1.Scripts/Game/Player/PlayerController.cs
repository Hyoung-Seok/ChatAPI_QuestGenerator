using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : UnitStateController
{
    [Header("Component")]
    public Rigidbody Rigidbody;
    public Transform PlayerTransform;
    public Transform CameraDir;
    
    [Header("State")]
    private PlayerBaseState _moveState;
    
    public void Init(PlayerMoveData data)
    {
        _moveState = new PlayerMoveState(this, data);
        
        ChangeMainState(_moveState);
    }
}
