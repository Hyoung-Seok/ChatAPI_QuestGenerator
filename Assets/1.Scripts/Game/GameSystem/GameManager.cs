using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Debug")] 
    public bool IsDebug = false;
    
    [Header("Player")]
    [SerializeField] private PlayerComponentData playerComponentData;
    [SerializeField] private PlayerStatus playerStatus;
    public WeaponManager WeaponManager;
    public PlayerController Player { get; private set; }

    [Header("PlayerCamera")] 
    [SerializeField] private PlayerCameraData playerCamData;
    public PlayerCameraController CameraController { get; private set; }

    [Header("Manager")] 
    [SerializeField] private InputManager inputManager;

    public static GameManager Instance { get; private set; }
    public PlayerComponentData PlayerComponent => playerComponentData;
    public CameraEffectController CameraEffect { get; private set; }
    
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
