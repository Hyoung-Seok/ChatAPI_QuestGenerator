using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private float disableTime;

    private float _curTime = 0.0f;
    private IObjectPool<ObjectPool> _managedPool;
    
    public void SetManagedPool(IObjectPool<ObjectPool> pool)
    {
        _managedPool = pool;
    }

    public virtual void OnEnableEvent(Vector3 dir)
    {
        
    }
    
    private void OnEnable()
    {
        _curTime = 0;
    }

    private void Update()
    {
        _curTime += Time.deltaTime;

        if (_curTime <= disableTime)
        {
            return;
        }
        
        gameObject.SetActive(false);
    }

    protected virtual void OnDisable()
    {
        _managedPool.Release(this);
    }
}
