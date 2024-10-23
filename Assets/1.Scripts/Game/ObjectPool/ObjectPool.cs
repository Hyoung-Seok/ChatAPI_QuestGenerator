using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private float disableTime;
    
    private IObjectPool<ObjectPool> _managedPool;
    private IEnumerator _disableRoutine;
    
    public void SetManagedPool(IObjectPool<ObjectPool> pool)
    {
        _managedPool = pool;
    }

    public virtual void OnEnableEvent() { }
    public virtual void OnEnableEvent(Vector3 dir) { }
    
    private void OnEnable()
    {
        _disableRoutine = DisableRoutine();
        StartCoroutine(_disableRoutine);
        
        OnEnableEvent();
    }
    
    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(disableTime);

        gameObject.SetActive(false);
    }

    protected virtual void OnDisable()
    {
        _managedPool.Release(this);
    }
}
