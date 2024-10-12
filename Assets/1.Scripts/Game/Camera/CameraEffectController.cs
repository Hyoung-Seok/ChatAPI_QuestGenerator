using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraEffectController
{
    private CinemachineVirtualCamera _idleCam;
    private CinemachineVirtualCamera _aimCam;
    private CinemachineImpulseSource _impulseSource;
    
    public CameraEffectController(PlayerCameraData data)
    {
        _idleCam = data.VirtualCameras[0];
        _aimCam = data.VirtualCameras[1];

        if (_aimCam.gameObject.TryGetComponent(out _impulseSource) == false)
        {
            Debug.Log("ImpulseSource Get Failed!");   
        }
    }

    public void TransitionCamera(EPlayerInputState state)
    {
        switch (state)
        {
            case EPlayerInputState.IDLE:
                _aimCam.gameObject.SetActive(false);
                break;
            
            case EPlayerInputState.AIM:
                _aimCam.gameObject.SetActive(true);
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
