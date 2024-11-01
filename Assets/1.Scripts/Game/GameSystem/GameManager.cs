using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Debug")] 
    public bool IsDebug = false;
    
    [Header("Player")]
    [SerializeField] private PlayerComponentData playerComponentData;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private WeaponManager weaponManager;

    [Header("PlayerCamera")] 
    [SerializeField] private PlayerCameraData playerCamData;

    [Header("Manager")] 
    [SerializeField] private AudioManager audioManager;
    [FormerlySerializedAs("uiManager")] [SerializeField] private PlayerUIManger playerUIManger;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestUIManager questUIManger;
    public NpcManager NpcManager { get; private set; }

    #region Property
    
    public static GameManager Instance { get; private set; }
    public PlayerComponentData PlayerComponent => playerComponentData;
    public WeaponManager WeaponManager => weaponManager;
    public PlayerController Player { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public CameraEffectController CameraEffect { get; private set; }
    public PlayerUIManger PlayerUIManger => playerUIManger;
    public AudioManager AudioManager => audioManager;
    public PlayerInput PlayerInput { get; private set; }
    public QuestManager QuestManager => questManager;
    public QuestUIManager QuestUIManager => questUIManger;
    
    #endregion
    
    private bool _isLockMouse = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Audio
        AudioManager.Init(5);
        
        // camera Init
        CameraController = new PlayerCameraController(playerCamData);
        CameraEffect = new CameraEffectController(playerCamData);
        
        // player Init
        Player = new PlayerController(playerStatus, playerComponentData);
        PlayerInput = playerComponentData.PlayerInput;
        
        NpcManager = new NpcManager();
        questManager.Init();
        QuestUIManager.Init();
        
        // Action Register
        PlayerInput.actions["Escape"].performed -= OnEscapeAction;
        PlayerInput.actions["Escape"].performed += OnEscapeAction;
        
        Cursor.lockState = CursorLockMode.Locked;
        _isLockMouse = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        _isLockMouse = false;
    }

    public void ChangePlayerState(string state)
    {
        switch (state)
        {
            case "Move":
                Player.ChangeMainState(Player.MoveState);
                break;
            
            case "Interaction":
                Player.ChangeMainState(Player.InteractionState);
                break;
        }
    }

    private void OnEscapeAction(InputAction.CallbackContext context)
    {
        if (context.performed != true)
        {
            return;
        }
        
        _isLockMouse = !_isLockMouse;
        Cursor.lockState = (_isLockMouse) ? CursorLockMode.Locked : CursorLockMode.None;
    }

    #region EventFunction
    
    private void Update()
    {
        // player
        CameraController.OnUpdate();
        Player.OnUpdate();
    }

    private void FixedUpdate()
    {
        Player.OnFixedUpdate();
    }

    private void LateUpdate()
    {

    }
    
    #endregion
}
