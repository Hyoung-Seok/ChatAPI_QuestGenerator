using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public bool IsPlayerInRange { get; private set; }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerInRange = false;
        }
    }
}
