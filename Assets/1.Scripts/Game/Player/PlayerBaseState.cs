using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : UnitBaseState
{
    protected static PlayerController Controller;

    public PlayerBaseState(PlayerController controller)
    {
        Controller = controller;
    }
}
