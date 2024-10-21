using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private UIContainer uiContainer;

    #region Property
    
    public static GameManager Instance { get; private set; }
    public PlayerComponentData PlayerComponent => playerComponentData;
    public WeaponManager WeaponManager => weaponManager;
    public PlayerController Player { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public CameraEffectController CameraEffect { get; private set; }
    public UIContainer UIContainer => uiContainer;
    public AudioManager AudioManager => audioManager;
    
    #endregion
    
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
        
        // camera Init
        CameraController = new PlayerCameraController(playerCamData);
        CameraEffect = new CameraEffectController(playerCamData);
        
        // player Init
        Player = new PlayerController(playerStatus, playerComponentData);
        
        // sound
        AudioManager.Init(5);
    }

    #region EventFunction
    
    private void Start()
    {
        
    }

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
}
