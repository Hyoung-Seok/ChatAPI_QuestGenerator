using UnityEngine;

public class ItemSpawnPos : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private Transform leftDown;
    [SerializeField] private Transform rightTop;

    public Vector2 LeftDown => new Vector2(leftDown.position.x, leftDown.position.z);
    public Vector2 RightTop => new Vector2(rightTop.position.x, rightTop.position.z);
    public float SpawnHeight => leftDown.position.y;
        
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        var rightDown = new Vector3(rightTop.position.x, leftDown.position.y, leftDown.position.z);
        var leftTop = new Vector3(leftDown.position.x, leftDown.position.y, rightTop.position.z);
        
        Gizmos.DrawLine(leftDown.position, rightDown);
        Gizmos.DrawLine(rightDown, rightTop.position);
        Gizmos.DrawLine(rightTop.position, leftTop);
        Gizmos.DrawLine(leftTop, leftDown.position);
    }
}
