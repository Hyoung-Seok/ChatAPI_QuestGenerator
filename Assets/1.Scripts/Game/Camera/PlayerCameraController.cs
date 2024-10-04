using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Value")] 
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float pitchMin = -50.0f;
    [SerializeField] private float pitchMax = 50.0f;

    [Header("Component")]
    [SerializeField] private Transform target;
    [SerializeField] private CinemachineBrain mainCam;

    private float _mouseX;
    private float _mouseY;
    private Vector2 _targetRotation = Vector2.zero;

    private void Update()
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
        
        transform.position = target.transform.position;
        target.rotation = Quaternion.LookRotation(CalLookDir(), Vector3.up);
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

    private Vector3 CalLookDir()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        
        var camForward = transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        var camRight = transform.right;
        camRight.y = 0;
        camRight.Normalize();

        var lookDir = camForward * vertical + camRight * horizontal;
        return lookDir.normalized;
    }
}
