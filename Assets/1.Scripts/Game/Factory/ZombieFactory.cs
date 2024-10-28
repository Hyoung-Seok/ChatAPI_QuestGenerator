using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class ZombieFactory : EnemyFactory
{
    [FormerlySerializedAs("normalZombieStatus")]
    [Header("Data")]
    [SerializeField] private EnemyStatus normalEnemyStatus;
    
    protected override EnemyBaseController CreateEnemy()
    {
        var obj = Instantiate(enemyPrefab, transform);
        obj.gameObject.SetActive(false);
        
        return new ZombieController(obj, normalEnemyStatus, SpawnerManager.Instance.GetPoolObject);
    }
}
