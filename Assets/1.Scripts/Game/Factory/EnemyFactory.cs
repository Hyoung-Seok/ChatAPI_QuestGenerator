using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class EnemyFactory : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] protected GameObject enemyPrefab;

    [Header("Spawn Value")] 
    [SerializeField] protected float spawnRange = 5.0f;
    [SerializeField] protected int firstCreateCount;
    [SerializeField] protected int firstSpawnCount;
    [SerializeField] private int maxTryCount = 10;

    private Queue<EnemyBaseController> _createEnemyContainer;
    private List<EnemyBaseController> _spawnEnemyContainer;
    protected abstract EnemyBaseController CreateEnemy();

    public void SetTargetTransformSpawnEnemy(Transform target)
    {
        _spawnEnemyContainer.ForEach(x => x.SetTargetTransform(target));
    }

    private void ReturnObjectToPool(EnemyBaseController obj)
    {
        obj.GameObject.SetActive(false);
        
        _spawnEnemyContainer.Remove(obj);
        _createEnemyContainer.Enqueue(obj);
    }

    private void SpawnEnemy(int count)
    {
        if (_createEnemyContainer.Count < count)
        {
            for (var i = 0; i < count * 2; ++i)
            {
                _createEnemyContainer.Enqueue(CreateEnemy());   
            }
        }

        for (var i = 0; i < count; ++i)
        {
            var obj = _createEnemyContainer.Dequeue();
            obj.SetPosition(GetRandomPosition());

            obj.ReturnAction -= ReturnObjectToPool;
            obj.ReturnAction += ReturnObjectToPool;
            
            obj.GameObject.SetActive(true);
            _spawnEnemyContainer.Add(obj);
        }
    }
    
    private void Start()
    {
        _createEnemyContainer = new Queue<EnemyBaseController>();
        _spawnEnemyContainer = new List<EnemyBaseController>();

        for (var i = 0; i < firstCreateCount; ++i)
        {
            _createEnemyContainer.Enqueue(CreateEnemy());   
        }
        
        SpawnEnemy(firstSpawnCount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnEnemy(1);
        }
        
        if (_spawnEnemyContainer.Count <= 0)
        {
            return;
        }

        for (var i = 0; i < _spawnEnemyContainer.Count; ++i)
        {
            _spawnEnemyContainer[i].OnUpdate();
        }
    }

    private Vector3 GetRandomPosition()
    {
        for (var i = 0; i < maxTryCount; ++i)
        {
            var randDir = Random.insideUnitCircle * spawnRange;
            var dir = new Vector3(randDir.x, 0, randDir.y) + transform.position;

            if (NavMesh.SamplePosition(dir, out var hit, 0.1f, NavMesh.AllAreas) == false)
            {
                continue;
            }
            
            if (Physics.CheckSphere(hit.position + Vector3.up * 0.5f, 0.5f) == false)
            {
                return hit.position;
            }
        }

        return transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
#endif
}
