using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : UnitStateController
{
    [Header("Component")] 
    [SerializeField] private Rigidbody playerRig;
    [SerializeField] private Transform cameraDir;

    [Header("State")] 
    [SerializeField] private PlayerBaseState idleState;
    [SerializeField] private PlayerBaseState moveState;

    #region Property

    public PlayerBaseState IdleState => idleState;
    public PlayerBaseState MoveState => moveState;
    public Rigidbody PlayerRig => playerRig;
    public Transform CameraDir => cameraDir;
    #endregion

    private void Awake()
    {
        moveState.InitPlayerController(this);
        ChangeMainState(moveState);
    }
}
