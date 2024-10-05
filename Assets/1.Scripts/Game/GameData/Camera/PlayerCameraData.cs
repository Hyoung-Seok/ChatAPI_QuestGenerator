using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraData : MonoBehaviour
{
    [Header("Value")] 
    public float RotationSpeed = 50.0f;
    public float PitchMin = -50.0f;
    public float PitchMax = 50.0f;
    
    [Header("Component")]
    public Transform Transform;
    public Transform TargetTf;
}
