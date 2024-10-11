using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraEffectController
{
    private readonly PlayerCameraData _data;
    private CinemachineImpulseSource _impulseSource;
    
    public CameraEffectController(PlayerCameraData data)
    {
        _data = data;

        if (_data.VirtualCameras[1].gameObject.TryGetComponent(out _impulseSource) == false)
        {
            Debug.Log("ImpulseSource Get Failed!");   
        }
    }

    public void TransitionCamera(EPlayerInputState state)
    {
        switch (state)
        {
            case EPlayerInputState.IDLE:
                _data.VirtualCameras[1].gameObject.SetActive(false);
                break;
            
            case EPlayerInputState.AIM:
                _data.VirtualCameras[1].gameObject.SetActive(true);
                break;
            
            default:
                return;
        }
    }

    public void ShakeCamera()
    {
        _impulseSource.GenerateImpulse();
    }
    
}
