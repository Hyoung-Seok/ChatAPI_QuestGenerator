using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraEffectController
{
    private readonly PlayerCameraData _data;
    private CinemachineBlendDefinition _cameraBlend;
    private CinemachineImpulseSource _impulseSource;
    
    public CameraEffectController(PlayerCameraData data)
    {
        _data = data;
        _cameraBlend = _data.MainCamera.m_DefaultBlend;

        if (_data.VirtualCameras[1].gameObject.TryGetComponent(out _impulseSource) == false)
        {
            Debug.Log("ImpulseSource Get Failed!");   
        }
    }

    public void TransitionCamera(ECameraState state)
    {
        switch (state)
        {
            case ECameraState.IDLE:
                _data.VirtualCameras[(int)ECameraState.AIM].gameObject.SetActive(false);
                break;
            
            case ECameraState.AIM:
                _data.VirtualCameras[(int)ECameraState.AIM].gameObject.SetActive(true);
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
