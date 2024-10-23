using System;
using UnityEngine;

public class OnPhysicsEvent : MonoBehaviour
{
    public event Action<float, HitPoint> OnTakeDamage;
    public event Action<float> OnHitTarget;

    private bool _isPlayerInRange = false;
    private AttackCollider _attackCollider;
    private float _damage = 0;

    private void Start()
    {
        _attackCollider = gameObject.GetComponentInChildren<AttackCollider>();
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    public void TakeDamage(float dmg, HitPoint point)
    {
        OnTakeDamage?.Invoke(dmg, point);
    }
    
    #region AnimationEvent

    public void AttackPlayer()
    {
        if (_attackCollider.IsPlayerInRange == false)
        {
            return;
        }
        
        OnHitTarget?.Invoke(_damage);
    }

    #endregion
}
