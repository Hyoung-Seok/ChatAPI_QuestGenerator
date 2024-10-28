using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected abstract void OnTriggerEnterEvent();
    protected abstract void OnTriggerStayEvent();
    protected abstract void OnTriggerExitEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        OnTriggerEnterEvent();
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        OnTriggerStayEvent();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }
        
        OnTriggerExitEvent();
    }
}
