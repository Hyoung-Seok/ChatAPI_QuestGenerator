using UnityEngine;
using UnityEngine.InputSystem;

public class Item : Interactable
{
    [Header("Data")] 
    [SerializeField] private ItemData data;
    [SerializeField] private GameObject interaction;
     
    protected override void OnTriggerEnterEvent()
    {
        GameManager.Instance.PlayerInput.actions["Interaction"].performed += OnInteractionEvent;
        interaction.SetActive(true);
    }

    protected override void OnTriggerStayEvent()
    {

    }

    protected override void OnTriggerExitEvent()
    {
        GameManager.Instance.PlayerInput.actions["Interaction"].performed -= OnInteractionEvent;
        interaction.SetActive(false);
    }

    private void OnInteractionEvent(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }
        
        GameManager.Instance.PlayerInventory.AddItem(data);
        GameManager.Instance.PlayerInput.actions["Interaction"].performed -= OnInteractionEvent;
        
        Destroy(gameObject);
    }
}
