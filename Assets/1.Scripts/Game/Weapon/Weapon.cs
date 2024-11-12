using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    
    private List<int> _magazineList;
    private int _curMagIndex = 0;
    public bool CanReload { get; private set; }

    public WeaponData WeaponData => weaponData;
    
    private IObjectPool<ObjectPool> _pool;
    private bool _canFire = true;
    private bool _isPressed = false;
    private float _curTime = 0;
    private Vector2 _screenCenter;
    private Camera _mainCam;
    private List<AudioClip> _gunSoundClip;

    private void Update()
    {
        if (_canFire == false)
        {
            _curTime += Time.deltaTime;

            if (_curTime < weaponData.FireRate)
            {
                return;
            }

            _canFire = true;
        }
        
        if (_isPressed == true && _canFire == true)
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

        _magazineList = new List<int>();
        for (var i = 0; i < 4; ++i)
        {
            _magazineList.Add(weaponData.Magazine);
        }
        CanReload = true;
        
        GameManager.Instance.PlayerUIManger.SetMagazineCountUI(_magazineList.Count - 1);
        GameManager.Instance.PlayerUIManger.SetMagazineInfoUI(weaponData.Magazine,weaponData.Magazine);
        GameManager.Instance.CameraController.SetRecoil(weaponData);
        
        var playerInput = GameManager.Instance.PlayerInput;
        playerInput.actions["Fire"].performed += OnFire;
        playerInput.actions["Fire"].canceled += OnFire;
        
        _mainCam = Camera.main;
        _screenCenter = new Vector2((float)Screen.width / 2, (float)Screen.height / 2);
    }
    
    public void Reload()
    {
        if (_magazineList.Count <= 1)
        {
            return;
        }
        
        if (_magazineList[_curMagIndex] <= 0)
        {
            _magazineList.RemoveAt(_curMagIndex);
            GameManager.Instance.PlayerUIManger.SetMagazineCountUI(_magazineList.Count - 1);

            if (_magazineList.Count - 1 <= 0)
            {
                CanReload = false;
            }
        }

        _curMagIndex = (_curMagIndex + 1 < _magazineList.Count) ? ++_curMagIndex : 0;
        GameManager.Instance.PlayerUIManger.SetCurrentBulletInfoUI(_magazineList[_curMagIndex]);
    }

    private void Fire()
    {
        if (_magazineList[_curMagIndex] <= 0)
        {
            return;
        }
        
        _pool.Get();
            
        muzzleEffect.Play();

        audioSource.clip = _gunSoundClip[Random.Range(0, _gunSoundClip.Count)];
        audioSource.Play();
            
        _canFire = false;
        _curTime = 0;
            
        ShootRayFormCenter();
            
        GameManager.Instance.CameraEffect.ShakeCamera(ECameraShake.RECOIL);
        GameManager.Instance.CameraController.IsRecoil = true;
        GameManager.Instance.PlayerUIManger.SetCurrentBulletInfoUI(--_magazineList[_curMagIndex]);
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
            hit.collider.gameObject.GetComponentInParent<OnPhysicsEvent>()?.TakeDamage(weaponData.Damage * 1.5f, _hitPoint);
            
            GameManager.Instance.AudioManager.PlaySound(ESoundType.EFFECT, "HeadShot", false);
            GameManager.Instance.PlayerUIManger.SetActiveCrossHair(true, true);
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        _isPressed = context.performed == true;

        if (context.canceled == true)
        {
            GameManager.Instance.CameraController.IsRecoil = false;
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
        obj.gameObject.transform.SetPositionAndRotation(cartridgeOutPos.position,
            weaponData.Cartridge.transform.rotation);
        
        obj.gameObject.SetActive(true);
        
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
