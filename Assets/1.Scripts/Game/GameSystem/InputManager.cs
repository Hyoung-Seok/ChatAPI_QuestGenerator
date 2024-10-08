using System;
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

    private PlayerController _controller;
    
    private void Start()
    {
        _controller = gameObject.GetComponentInChildren<PlayerController>();
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(equippedKey))
        {
            equippedAction?.Invoke();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _controller.CurCameraState = ECameraState.AIM;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            _controller.CurCameraState = ECameraState.IDLE;
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLateUpdate()
    {

    }
}
