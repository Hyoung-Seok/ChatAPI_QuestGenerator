using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        if (Controller.CurInputState == EPlayerInputState.AIM)
        {
            GameManager.Instance.UIContainer.SetActiveCrossHair(false);
            GameManager.Instance.CameraController.IsRecoil = false;
            
            Controller.ChangePlayerInputState(EPlayerInputState.IDLE);
        }
        
        SpawnerManager.Instance.ResetAllEnemyTargetTransform();
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Controller.ResetPlayer();
        }
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnLateUpdate()
    {

    }

    public override void Exit()
    {
        
    }
}
