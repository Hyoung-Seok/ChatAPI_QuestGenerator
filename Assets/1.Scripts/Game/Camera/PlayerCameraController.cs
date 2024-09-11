using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Value")] 
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float pitchMin = -50.0f;
    [SerializeField] private float pitchMax = 50.0f;

    private float _mouseX;
    private float _mouseY;
    private Vector2 _targetRotation = Vector2.zero;

    private void LateUpdate()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        _mouseX = Input.GetAxis("Mouse Y");
        _mouseY = Input.GetAxis("Mouse X");

        _targetRotation.x += _mouseX * rotationSpeed * Time.deltaTime;
        _targetRotation.y += _mouseY * rotationSpeed * Time.deltaTime;

        _targetRotation.x = Mathf.Clamp(_targetRotation.x, pitchMin, pitchMax);
        _targetRotation.y = Clamp360();

        var targetAngle = Quaternion.Euler(_targetRotation.x, _targetRotation.y, 0);
        transform.rotation = targetAngle;
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
