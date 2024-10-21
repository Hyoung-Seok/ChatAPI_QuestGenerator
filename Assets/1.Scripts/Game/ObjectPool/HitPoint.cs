using UnityEngine;

public class HitPoint
{
    public Vector3 HitPosition = Vector3.zero;
    public Vector3 HitNormal = Vector3.zero;

    public void Init(RaycastHit hit)
    {
        HitPosition = hit.point;
        HitNormal = hit.normal;
    }
}
