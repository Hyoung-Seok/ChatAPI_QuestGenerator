using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")] 
    [SerializeField] private PlayerMoveData playerMoveData;
    [SerializeField] private PlayerController playerController;

    [Header("PlayerCamera")] 
    [SerializeField] private PlayerCameraData playerCamData;
    private PlayerCameraController _playerCameraController;

    [Header("Manager")] 
    [SerializeField] private InputManager inputManager;
    
    private void Awake()
    {
        // camera Init
        _playerCameraController = new PlayerCameraController(playerCamData);
        
        // player Init
        playerController.Init(playerMoveData);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // manager
        inputManager.OnUpdate();
        
        // player
        _playerCameraController.OnUpdate();
        playerController.OnUpdate();   
    }

    private void FixedUpdate()
    {
        playerController.OnFixedUpdate();
    }

    private void LateUpdate()
    {
    }
}
