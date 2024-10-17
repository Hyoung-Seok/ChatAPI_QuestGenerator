using UnityEngine;

public class InputManager : MonoBehaviour, IEventFunction
{
    [Header("Player Input")] 
    [SerializeField] private KeyCode equippedKey;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    private PlayerController _controller;
    
    private void Start()
    {
        _controller = GameManager.Instance.Player;
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
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLateUpdate()
    {

    }
}
