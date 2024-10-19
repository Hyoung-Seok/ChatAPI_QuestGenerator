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

    private IObjectPool<ObjectPool> _effectPool;

    private void Awake()
    {
        _effectPool = new ObjectPool<ObjectPool>(CreateHitEffect, GetObject, ReturnObject,
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
        return _effectPool.Get();
    }
    
    private ObjectPool CreateHitEffect()
    {
        var effect = Instantiate(hitEffect, effectParent).GetComponent<ObjectPool>();
        effect.SetManagedPool(_effectPool);

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
