using UnityEngine;

public class PlayerCameraController : IEventFunction
{
    private PlayerCameraData _data;
    
    private float _mouseX;
    private float _mouseY;
    private Vector2 _targetRotation = Vector2.zero;

    public PlayerCameraController(PlayerCameraData data)
    {
        _data = data;
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

    private void CameraMove()
    {
        _mouseX = Input.GetAxis("Mouse Y");
        _mouseY = Input.GetAxis("Mouse X");
        
        _targetRotation.x += _mouseX * _data.RotationSpeed * Time.deltaTime;
        _targetRotation.y += _mouseY * _data.RotationSpeed  * Time.deltaTime;

        _targetRotation.x = Mathf.Clamp(_targetRotation.x, _data.PitchMin, _data.PitchMax);
        _targetRotation.y = Clamp360();

        var targetAngle = Quaternion.Euler(_targetRotation.x, _targetRotation.y, 0);
        _data.Transform.rotation = targetAngle;
        
        _data.Transform.position = _data.TargetTf.position;
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
