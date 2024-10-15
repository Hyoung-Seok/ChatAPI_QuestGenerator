using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCameraData : MonoBehaviour
{
    [Header("Value")] 
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float pitchMin = -50.0f;
    [SerializeField] private float pitchMax = 50.0f;
    
    [Header("Component")]
    [SerializeField] private new Transform transform;
    [SerializeField] private Transform targetTf;

    [Header("Camera List")] 
    [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;
    
    public float RotationSpeed => rotationSpeed;
    public float PitchMin => pitchMin;
    public float PitchMax => pitchMax;
    public Transform Transform => transform;
    public Transform TargetTf => targetTf;
    public List<CinemachineVirtualCamera> VirtualCameras => virtualCameras;
}

