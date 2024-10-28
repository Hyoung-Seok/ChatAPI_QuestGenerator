using System;
using UnityEngine.InputSystem;

public class NpcManager
{
    public event Action EnterInteraction = null;
    public event Action ExitInteraction = null;
    
    public NpcManager()
    {
        var playerInput = GameManager.Instance.PlayerInput;

        playerInput.actions["Interaction"].performed += OnEnterInteraction;
        playerInput.actions["Escape"].performed += OnExitInteraction;
    }
    
    private void OnEnterInteraction(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }
        
        EnterInteraction?.Invoke();
    }

    private void OnExitInteraction(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }
        
        ExitInteraction?.Invoke();
    }
}
