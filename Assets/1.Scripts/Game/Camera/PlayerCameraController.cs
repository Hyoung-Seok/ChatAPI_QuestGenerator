using UnityEngine;

public class PlayerCameraController : IEventFunction
{
    private float _mouseX;
    private float _mouseY;
    private readonly float _rotationSpeed;
    private readonly float _pitchMin;
    private readonly float _pitchMax;
    private readonly Transform _transform;
    private readonly Transform _targetTf;
    private Vector2 _targetRotation = Vector2.zero;
    
    // recoil value
    public bool IsRecoil = false;
    private float _recoilX;
    private float _recoilY;

    public PlayerCameraController(PlayerCameraData data)
    {
        _rotationSpeed = data.RotationSpeed;
        _pitchMin = data.PitchMin;
        _pitchMax = data.PitchMax;
        _transform = data.Transform;
        _targetTf = data.TargetTf;
    }
    
    public void OnUpdate()
    {
        CameraMove();
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnLateUpdate()
    {

    }

    public void SetRecoil(WeaponData data)
    {
        _recoilX = data.RecoilX;
        _recoilY = data.RecoilY;
    }

    private void CameraMove()
    {
        _mouseX = Input.GetAxis("Mouse Y");
        _mouseY = Input.GetAxis("Mouse X");
        
        _targetRotation.x += _mouseX * _rotationSpeed * Time.deltaTime;
        _targetRotation.y += _mouseY * _rotationSpeed * Time.deltaTime;

        if (IsRecoil == true)
        {
            ApplyRecoil();
        }

        _targetRotation.x = Mathf.Clamp(_targetRotation.x, _pitchMin, _pitchMax);
        _targetRotation.y = Clamp360();

        var targetAngle = Quaternion.Euler(_targetRotation.x, _targetRotation.y, 0);
        _transform.rotation = targetAngle;

        _transform.position = _targetTf.position;
    }

    private void ApplyRecoil()
    {
        _targetRotation.x -= Random.Range(_recoilX, _recoilX * 2);
        _targetRotation.y += Random.Range(-_recoilY, _recoilY);
    }
    
    private float Clamp360()
    {
        var result = _targetRotation.y - Mathf.Ceil(_targetRotation.y / 360f) * 360f;

        if (result < 0)
        {
            result += 360f;
        }

        return result;
    }
}
