using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform cartridgeOutPos;

    [Header("Effect")] 
    [SerializeField] private ParticleSystem muzzleEffect;
    private HitPoint _hitPoint;

    public WeaponData WeaponData => weaponData;
    
    private IObjectPool<ObjectPool> _pool;
    private bool _canFire = true;
    private float _curTime = 0;
    private Vector2 _screenCenter;
    private Camera _mainCam;
    private List<AudioClip> _gunSoundClip;

    private void Start()
    {
        Init();
        
        _mainCam = Camera.main;
        _screenCenter = new Vector2((float)Screen.width / 2, (float)Screen.height / 2);
    }

    private void Update()
    {
        if (Input.GetButtonUp("Fire1") == true)
        {
            GameManager.Instance.CameraController.IsRecoil = false;
        }
        
        if (_canFire == false)
        {
            _curTime += Time.deltaTime;

            if (_curTime < weaponData.FireRate)
            {
                return;
            }

            _canFire = true;
        }
        
        if (Input.GetButton("Fire1") == true && _canFire == true)
        {
            Fire();
        }
    }

    public void Init()
    {
        _pool = new ObjectPool<ObjectPool>(CreateObjectPool, GetObject, ReturnObject,
            OnDestroyPoolObject, maxSize: weaponData.Magazine + 10);
        
        _hitPoint = new HitPoint();
        
        muzzleEffect.Stop();

        _gunSoundClip = new List<AudioClip>();
        _gunSoundClip = weaponData.GunSound;
        
        GameManager.Instance.CameraController.SetRecoil(weaponData);
    }
    
    public void Reload()
    {
        
    }

    private void Fire()
    {
        _pool.Get();
            
        muzzleEffect.Play();

        audioSource.clip = _gunSoundClip[Random.Range(0, _gunSoundClip.Count)];
        audioSource.Play();
            
        _canFire = false;
        _curTime = 0;
            
        ShootRayFormCenter();
            
        GameManager.Instance.CameraEffect.ShakeCamera(ECameraShake.RECOIL);
        GameManager.Instance.CameraController.IsRecoil = true;
    }

    private void ShootRayFormCenter()
    {
        var ray = _mainCam.ScreenPointToRay(_screenCenter);

        if (Physics.Raycast(ray, out var hit, 100.0f) == false)
        {
            return;
        }

        if (hit.collider.CompareTag("Enemy"))
        {
            _hitPoint.Init(hit);
            hit.collider.gameObject.GetComponent<OnPhysicsEvent>()?.TakeDamage(weaponData.Damage, _hitPoint);
            
        }
        else if (hit.collider.CompareTag("Head"))
        {
            _hitPoint.Init(hit);
            hit.collider.gameObject.GetComponent<OnPhysicsEvent>()?.TakeDamage(weaponData.Damage * 1.5f, _hitPoint);
            
            GameManager.Instance.AudioManager.PlaySound(ESoundType.EFFECT, "HeadShot", false);
            GameManager.Instance.UIContainer.SetActiveCrossHair(true, true);
        }
    }

    #region ObjectPool

    private ObjectPool CreateObjectPool()
    {
        var obj = Instantiate(weaponData.Cartridge, cartridgeOutPos).GetComponent<Cartridge>();
        obj.SetManagedPool(_pool);

        return obj;
    }
    
    private void GetObject(ObjectPool obj)
    {
        obj.gameObject.SetActive(true);
        
        obj.gameObject.transform.SetPositionAndRotation(cartridgeOutPos.position,
            weaponData.Cartridge.transform.rotation);
        obj.OnEnableEvent(cartridgeOutPos.forward);
    }

    private void ReturnObject(ObjectPool obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(ObjectPool obj)
    {
        Destroy(obj.gameObject);
    }

    #endregion
}
