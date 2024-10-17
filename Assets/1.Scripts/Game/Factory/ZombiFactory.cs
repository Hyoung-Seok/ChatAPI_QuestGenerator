using UnityEngine;

public class ZombiFactory : EnemyFactory
{
    [Header("Data")]
    [SerializeField] private ZombieStatus normalZombieStatus;
    
    protected override EnemyBaseController CreateEnemy()
    {
        var obj = Instantiate(enemyPrefab, transform);
        obj.gameObject.SetActive(false);
        
        return new ZombieController(obj, normalZombieStatus);
    }
}
