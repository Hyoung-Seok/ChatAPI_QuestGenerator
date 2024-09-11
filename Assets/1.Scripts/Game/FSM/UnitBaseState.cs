using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBaseState : MonoBehaviour
{
    public abstract void Enter();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnLateUpdate();
    public abstract void Exit();
}
