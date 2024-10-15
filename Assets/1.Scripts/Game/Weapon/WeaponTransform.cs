using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponTransform
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Vector3 scale;

    public Vector3 Position => position;
    public Vector3 Rotation => rotation;
    public Vector3 Scale => scale;
}
