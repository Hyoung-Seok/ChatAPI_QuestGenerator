using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

public class Cartridge : ObjectPool
{
    [Header("Force")]
    [SerializeField] private float minDischargeForce;
    [SerializeField] private float maxDischargeForce;

    [Header("Angle")]
    [SerializeField] private float randomAngle = 10;
    
    [Header("Component")]
    [SerializeField] private Rigidbody rigidBody;
    
    public override void OnEnableEvent(Vector3 dir)
    {
        var force = Random.Range(minDischargeForce, maxDischargeForce);
        var randAngle = Quaternion.Euler(0, Random.Range(-randomAngle, randomAngle), 0);
        
        rigidBody.AddForce(randAngle * dir * force, ForceMode.VelocityChange);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        rigidBody.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
