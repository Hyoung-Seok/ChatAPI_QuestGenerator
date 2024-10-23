using UnityEngine;

public class InputManager : MonoBehaviour, IEventFunction
{
    [Header("Player Input")] 
    [SerializeField] private KeyCode equippedKey;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    private PlayerController _controller;
    private bool _isLockMouse = false;
    
    private void Start()
    {
        _controller = GameManager.Instance.Player;
        
        Cursor.lockState = CursorLockMode.Locked;
        _isLockMouse = true;
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(equippedKey))
        {
            _controller.ChangePlayerInputState(EPlayerInputState.EQUIPPED);
        }
        
        if (Input.GetButtonDown("Fire2") && _controller.IsEquipped == true)
        {
            GameManager.Instance.UIManager.SetActiveCrossHair(true);
            _controller.ChangePlayerInputState(EPlayerInputState.AIM);
        }

        if (Input.GetButtonUp("Fire2") && _controller.IsEquipped == true)
        {
            GameManager.Instance.UIManager.SetActiveCrossHair(false);
            GameManager.Instance.CameraController.IsRecoil = false;
            
            _controller.ChangePlayerInputState(EPlayerInputState.IDLE);
        }

        if (Input.GetButtonDown("Reloading") && _controller.IsEquipped == true && _controller.CurInputState != EPlayerInputState.AIM)
        {
            _controller.ChangePlayerInputState(EPlayerInputState.RELOADING);
        }
        
        if (Input.GetButtonDown("Cancel") == true)
        {
            _isLockMouse = !_isLockMouse;
            Cursor.lockState = (_isLockMouse) ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLateUpdate()
    {

    }
}
