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
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;

    #region Property
    
    public static GameManager Instance { get; private set; }
    public PlayerComponentData PlayerComponent => playerComponentData;
    public WeaponManager WeaponManager => weaponManager;
    public PlayerController Player { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public CameraEffectController CameraEffect { get; private set; }
    public UIManager UIManager => uiManager;
    public AudioManager AudioManager => audioManager;
    public PlayerInput PlayerInput { get; private set; }
    
    #endregion
    
    private IEnumerator _healthWarningEffect;
    private bool _isPlaying = false;
    
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
        
        // sound
        AudioManager.Init(5);
        
        // camera Init
        CameraController = new PlayerCameraController(playerCamData);
        CameraEffect = new CameraEffectController(playerCamData);

        _healthWarningEffect = CameraEffect.HealthWarningEffect();
        
        // player Init
        Player = new PlayerController(playerStatus, playerComponentData);
        PlayerInput = playerComponentData.PlayerInput;
    }

    #region EventFunction
    
    private void Update()
    {
        // manager
        inputManager.OnUpdate();
        
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

    #region Coroutine

    public void StartHitCameraEffect()
    {
        StartCoroutine(CameraEffect.HitCameraEffect());
    }

    public void StartHealthEffect()
    {
        if (_isPlaying == true)
        {
            return;
        }
        
        StartCoroutine(_healthWarningEffect);
        _isPlaying = true;
    }

    public void StopHealthEffect()
    {
        StopCoroutine(_healthWarningEffect);
        CameraEffect.ResetVignetteValue();
        _isPlaying = false;
    }

    #endregion
}
