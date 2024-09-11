using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateController : MonoBehaviour
{
    protected UnitBaseState MainState;
    protected UnitBaseState SubState;

    public void ChangeMainState(UnitBaseState state)
    {
        if (MainState != null)
        {
            MainState.Exit();
        }

        MainState = state;
        MainState.Enter();
        
        Debug.Log($"State Change = {state}");
    }

    public void ChangeSubState(UnitBaseState state)
    {
        if (SubState != null)
        {
            SubState.Exit();
        }

        SubState = state;
        SubState.Enter();
        
        Debug.Log($"State Change = {state}");
    }

    private void Update()
    {
        if (MainState != null)
        {
            MainState.OnUpdate();
        }
        
        if (SubState != null)
        {
            SubState.OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (MainState != null)
        {
            MainState.OnUpdate();
        }
        
        if (SubState != null)
        {
            SubState.OnUpdate();
        }
    }

    private void LateUpdate()
    {
        if (MainState != null)
        {
            MainState.OnUpdate();
        }
        
        if (SubState != null)
        {
            SubState.OnUpdate();
        }
    }
}
