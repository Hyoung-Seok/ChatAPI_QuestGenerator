using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnerManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Transform effectParent;
    [SerializeField] private GameObject hitEffect;
    
    [Header("Current Spawner")]
    [SerializeField, ReadOnly] private List<EnemyFactory> _spawnerList;
    public static SpawnerManager Instance { get; private set; }
    private IObjectPool<ObjectPool> _effectPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _spawnerList = new List<EnemyFactory>();
        for (var i = 1; i < transform.childCount; ++i)
        {
            _spawnerList.Add(transform.GetChild(i).GetComponent<EnemyFactory>());
        }
        
        _effectPool = new ObjectPool<ObjectPool>(CreateHitEffect, GetObject, ReturnObject,
            OnDestroyPoolObject, maxSize: 20);
    }

    public void ResetAllEnemyTargetTransform(Transform target = null)
    {
        if (target == null)
        {
            _spawnerList.ForEach(x => x.SetTargetTransformSpawnEnemy(effectParent));
            return;
        }
        
        _spawnerList.ForEach(x => x.SetTargetTransformSpawnEnemy(target));
    }
    
    public ObjectPool GetPoolObject()
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
