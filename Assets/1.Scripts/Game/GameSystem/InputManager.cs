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
        
        if (Input.GetKeyDown(runKey))
        {
            _controller.ChangePlayerInputState(EPlayerInputState.RUN);
        }
        
        if (Input.GetKeyUp(runKey))
        {
            _controller.ChangePlayerInputState(EPlayerInputState.WALK);
        }
        
        if (Input.GetButtonDown("Fire2") && _controller.IsEquipped == true)
        {
            _controller.ChangePlayerInputState(EPlayerInputState.AIM);
        }

        if (Input.GetButtonUp("Fire2") && _controller.IsEquipped == true)
        {
            _controller.ChangePlayerInputState(EPlayerInputState.IDLE);
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
