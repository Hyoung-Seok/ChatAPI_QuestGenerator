using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum ECameraShake
{
    RECOIL,
    HIT
}

public class CameraEffectController
{
    private readonly Volume _volume;
    private readonly Vignette _vignette;
    
    private CinemachineVirtualCamera _idleCam;
    private readonly CinemachineVirtualCamera _aimCam;
    private readonly CinemachineImpulseSource _idleImpulseSource;
    private readonly CinemachineImpulseSource[] _aimImpulseSource;
    
    // Vignette
    private readonly float _maxIntensity;
    private readonly float _fadeSpeed;
    private bool _isHealthWarning;
    private float _curTime;
    
    public CameraEffectController(PlayerCameraData data)
    {
        _idleCam = data.VirtualCameras[0];
        _aimCam = data.VirtualCameras[1];

        _idleImpulseSource = _idleCam.GetComponent<CinemachineImpulseSource>();
        _aimImpulseSource = _aimCam.gameObject.GetComponents<CinemachineImpulseSource>();

        _volume = data.Volume;
        if (_volume.profile.TryGet<Vignette>(out _vignette) == false)
        {
            Debug.Log("Vignette 없다!");
        }

        _fadeSpeed = data.FadeSpeed;
        _maxIntensity = data.MaxVignetteIntensity;
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

    public async UniTask HitCameraEffectRoutine()
    {
        _vignette.intensity.value = _maxIntensity;
        
        while (_vignette.intensity.value > 0)
        {
            _vignette.intensity.value =
                Mathf.MoveTowards(_vignette.intensity.value, 0, Time.deltaTime * _fadeSpeed);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    public void StartHealthWarningEffect()
    {
        _isHealthWarning = true;
        HealthWarningEffect().Forget();
    }

    public void StopHealthWarningEffect()
    {
        _isHealthWarning = false;
    }

    private async UniTask HealthWarningEffect()
    {
        while (_isHealthWarning)
        {
            _curTime += Time.deltaTime * _fadeSpeed * 2;
            var intensity = Mathf.Abs(Mathf.Sin(_curTime) * _maxIntensity);

            _vignette.intensity.value = intensity;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        _vignette.intensity.value = 0;
    }
}
