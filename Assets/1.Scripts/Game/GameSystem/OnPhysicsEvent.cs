using System;
using UnityEngine;

public class OnPhysicsEvent : MonoBehaviour
{
    public event Action<float, HitPoint> OnTakeDamage;
    public event Action<float> OnHitTarget;

    private bool _isPlayerInRange = false;
    private float _damage = 0;

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public void TakeDamage(float dmg, HitPoint point)
    {
        OnTakeDamage?.Invoke(dmg, point);
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
        
        OnHitTarget?.Invoke(_damage);
    }

    #endregion
}
