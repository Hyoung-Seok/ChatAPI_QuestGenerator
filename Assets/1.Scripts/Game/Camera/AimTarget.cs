using UnityEngine;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float maxDistance = 100.0f;
    
    private Vector3 _screenCenter;
    private void Start()
    {
        _screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    private void Update()
    {
        var ray = camera.ScreenPointToRay(_screenCenter);
        
        if (Physics.Raycast(ray, out var rayCastHit, maxDistance, mask))
        {
            transform.position = rayCastHit.point;
        }
    }
}
