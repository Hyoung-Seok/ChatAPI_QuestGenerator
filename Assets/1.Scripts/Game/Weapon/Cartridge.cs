using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

public class Cartridge : MonoBehaviour
{
    [Header("Force")]
    [SerializeField] private float minDischargeForce;
    [SerializeField] private float maxDischargeForce;

    [Header("Angle")]
    [SerializeField] private float randomAngle = 10;

    [Header("Time")] 
    [SerializeField] private float disableTime;
    
    [Header("Component")]
    [SerializeField] private Rigidbody rigidBody;

    public Action<Cartridge> ReturnAction;
    
    public void StartCellDischarge(Vector3 dir)
    {
        var force = Random.Range(minDischargeForce, maxDischargeForce);
        var randAngle = Quaternion.Euler(0, Random.Range(-randomAngle, randomAngle), 0);
        
        rigidBody.AddForce(randAngle * dir * force, ForceMode.Impulse);
        
        Invoke(nameof(ReturnQueue), disableTime);
    }

    private void ReturnQueue()
    {
        ReturnAction?.Invoke(this);
    }
}
