using System;
using UnityEngine;

public class OnPhysicsEvent : MonoBehaviour
{
    public Action<float, HitPoint> OnHitFunc;

    private bool _isPlayerInRange = false;
    private float _damage = 0;

    public void SetDamage(float damage)
    {
        _damage = damage;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        _isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        _isPlayerInRange = false;
    }

    #region AnimationEvent

    public void AttackPlayer()
    {
        if (_isPlayerInRange == false)
        {
            return;
        }
        
        GameManager.Instance.Player.PlayerDamaged(_damage);
    }

    #endregion
}
