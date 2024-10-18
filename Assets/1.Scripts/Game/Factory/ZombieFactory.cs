using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ZombieFactory : EnemyFactory
{
    [Header("Data")]
    [SerializeField] private ZombieStatus normalZombieStatus;
    [SerializeField] private Transform effectParent;
    [SerializeField] private GameObject hitEffect;

    private IObjectPool<ObjectPool> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<ObjectPool>(CreateHitEffect, GetObject, ReturnObject,
            OnDestroyPoolObject, maxSize: 20);
    }

    protected override EnemyBaseController CreateEnemy()
    {
        var obj = Instantiate(enemyPrefab, transform);
        obj.gameObject.SetActive(false);
        
        return new ZombieController(obj, normalZombieStatus, GetPoolObject);
    }

    private ObjectPool GetPoolObject()
    {
        return _pool.Get();
    }
    
    private ObjectPool CreateHitEffect()
    {
        var effect = Instantiate(hitEffect, effectParent).GetComponent<ObjectPool>();
        effect.SetManagedPool(_pool);

        return effect;
    }

    private void GetObject(ObjectPool obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void ReturnObject(ObjectPool obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(ObjectPool obj)
    {
        Destroy(obj.gameObject);
    }
}
