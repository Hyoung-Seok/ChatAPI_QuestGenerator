using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiFactory : EnemyFactory
{
    [Header("Component")] 
    [SerializeField] private Transform enemyContainer;
    
    [Header("Prefabs")] 
    [SerializeField] private GameObject normalZombie;

    [Header("Data")]
    [SerializeField] private ZombieStatus normalZombieStatus;
    
    public override EnemyBaseController CreateEnemy()
    {
        var obj = Instantiate(normalZombie, enemyContainer);

        var nav = obj.GetComponent<NavMeshAgent>();
        var rig = obj.GetComponent<Rigidbody>();
        var ani = obj.GetComponent<Animator>();
        var tf = obj.transform;

        var enemyComponent = new EnemyComponent(nav, rig, tf, ani);
        return new ZombieController(enemyComponent, normalZombieStatus);
    }

    private List<EnemyBaseController> _controllers;

    private void Start()
    {
        _controllers = new List<EnemyBaseController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _controllers.Add(CreateEnemy());
        }
    }
}
