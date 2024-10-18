using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private float disableTime;

    private float _curTime = 0.0f;
    private IObjectPool<ObjectPool> _managedPool;

    public void SetManagedPool(IObjectPool<ObjectPool> pool)
    {
        _managedPool = pool;
    }

    private void OnEnable()
    {
        _curTime = 0;
    }

    private void Update()
    {
        _curTime += Time.deltaTime;

        if (_curTime <= disableTime)
        {
            return;
        }
        
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _managedPool.Release(this);
    }
}
