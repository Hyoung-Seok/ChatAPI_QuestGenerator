using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventFunction
{
    public void OnUpdate();
    public void OnFixedUpdate();
    public void OnLateUpdate();
}