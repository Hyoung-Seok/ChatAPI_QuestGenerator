using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum ECameraShake
{
    RECOIL,
    HIT
}

public class CameraEffectController
{
    private CinemachineVirtualCamera _idleCam;
    private readonly CinemachineVirtualCamera _aimCam;
    private readonly CinemachineImpulseSource _idleImpulseSource;
    private readonly CinemachineImpulseSource[] _aimImpulseSource;
    
    public CameraEffectController(PlayerCameraData data)
    {
        _idleCam = data.VirtualCameras[0];
        _aimCam = data.VirtualCameras[1];

        _idleImpulseSource = _idleCam.GetComponent<CinemachineImpulseSource>();
        _aimImpulseSource = _aimCam.gameObject.GetComponents<CinemachineImpulseSource>();
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

    public void ShakeCamera(ECameraShake type, float force = 1)
    {
        switch (type)
        {
            case ECameraShake.RECOIL:
                _aimImpulseSource[0].GenerateImpulseWithForce(force);
                break;
            
            case ECameraShake.HIT:
                if (_aimCam.enabled == true)
                {
                    _aimImpulseSource[1].GenerateImpulse(force);
                    return;
                }
                _idleImpulseSource.GenerateImpulseWithForce(force);
                break;
            
            default:
                return;
        }
    }
    
}
