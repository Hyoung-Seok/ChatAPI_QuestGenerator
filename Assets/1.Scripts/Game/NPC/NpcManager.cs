using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NpcManager
{
    public event Action EnterInteraction = null;
    public event Action ExitInteraction = null;
    
    private readonly Dictionary<string, NpcController> _npcDic;
    
    public NpcManager()
    {
        var playerInput = GameManager.Instance.PlayerInput;
        _npcDic = new Dictionary<string, NpcController>();

        playerInput.actions["Interaction"].performed += OnEnterInteraction;
        playerInput.actions["Escape"].performed += OnExitInteraction;
    }

    public void AddNpcControllerInDictionary(string name, NpcController controller)
    {
        _npcDic.Add(name, controller);
    }

    public NpcController GetNpcControllerOrNull(string key)
    {
        return _npcDic.GetValueOrDefault(key);
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
