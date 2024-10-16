using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [Header("Debug")] 
    public bool IsDebug = false;
    
    public static TestManager Instance { get; private set; }
    public List<EnemyBaseController> ZombieControllers { get; private set; }
    public Transform Target;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ZombieControllers = new List<EnemyBaseController>();
    }

    private void Update()
    {
        if (ZombieControllers.Count <= 0)
        {
            return;
        }

        foreach (var zombie in ZombieControllers)
        {
            zombie.OnUpdate();
        }
    }
}
