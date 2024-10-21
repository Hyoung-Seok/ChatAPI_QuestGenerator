using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class PlayerCameraData : MonoBehaviour
{
    [Header("Value")] 
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float pitchMin = -50.0f;
    [SerializeField] private float pitchMax = 50.0f;
    
    [Header("Component")]
    [SerializeField] private new Transform transform;
    [SerializeField] private Transform targetTf;
    [SerializeField] private Volume volume;

    [Header("Camera List")] 
    [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;

    [Header("Camera Effect")] 
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    [SerializeField] private float fadeSpeed = 0.2f;
    
    public float RotationSpeed => rotationSpeed;
    public float PitchMin => pitchMin;
    public float PitchMax => pitchMax;
    public Transform Transform => transform;
    public Transform TargetTf => targetTf;
    public Volume Volume => volume;
    public List<CinemachineVirtualCamera> VirtualCameras => virtualCameras;
    public float MaxVignetteIntensity => maxVignetteIntensity;
    public float FadeSpeed => fadeSpeed;
}

