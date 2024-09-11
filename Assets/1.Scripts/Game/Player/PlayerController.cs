using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitStateController
{
    [Header("state")] 
    [SerializeField] private PlayerBaseState moveState;
    [SerializeField] private PlayerBaseState attackState;

    public PlayerBaseState MoveState => moveState;
    public PlayerBaseState AttackState => attackState;

    private void Awake()
    {
        moveState.InitPlayerController(this);
        ChangeMainState(moveState);
    }
}
