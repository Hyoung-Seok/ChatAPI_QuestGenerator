using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ZombieFactory : EnemyFactory
{
    [Header("Data")]
    [SerializeField] private ZombieStatus normalZombieStatus;
    
    protected override EnemyBaseController CreateEnemy()
    {
        var obj = Instantiate(enemyPrefab, transform);
        obj.gameObject.SetActive(false);
        
        return new ZombieController(obj, normalZombieStatus, SpawnerManager.Instance.GetPoolObject);
    }
}
