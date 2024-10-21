using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    private readonly int _deathKey = Animator.StringToHash("IsDead");
    
    public PlayerDeathState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        Controller.Animator.SetBool(_deathKey, true);
        SpawnerManager.Instance.ResetAllEnemyTargetTransform();
    }

    public override void OnUpdate()
    {

    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnLateUpdate()
    {

    }

    public override void Exit()
    {
        Controller.Animator.SetBool(_deathKey, false);
    }
}
