using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour, IEventFunction
{
    [Header("Player Input")] 
    [SerializeField] private KeyCode equippedKey;

    [Header("Event")]
    [SerializeField] private UnityEvent equippedAction;

    public void OnUpdate()
    {
        if (Input.GetKeyDown(equippedKey))
        {
            equippedAction?.Invoke();
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLateUpdate()
    {

    }
}
