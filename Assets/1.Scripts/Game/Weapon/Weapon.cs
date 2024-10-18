using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private WeaponSound sound;
    [SerializeField] private Transform cartridgeOutPos;

    [Header("Effect")] 
    [SerializeField] private ParticleSystem muzzleEffect;

    public WeaponData WeaponData => weaponData;
    
    private Queue<Cartridge> _shells;
    
    private bool _canFire = true;
    private float _curTime = 0;
    private Vector2 _screenCenter;
    private Camera _mainCam;

    private void Start()
    {
        Init();
        GameManager.Instance.CameraController.SetRecoil(weaponData);
        
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
            var cartridge = GetCartridge();
            cartridge.StartCellDischarge(cartridgeOutPos.forward);
            
            muzzleEffect.Play();
            sound.PlayFireSound();
            
            _canFire = false;
            _curTime = 0;
            
            ShootRayFormCenter();
            
            GameManager.Instance.CameraEffect.ShakeCamera();
            GameManager.Instance.CameraController.IsRecoil = true;
        }
    }

    public void Init()
    {
        _shells = new Queue<Cartridge>();
        CreateCartridge();

        muzzleEffect.Stop();
    }
    
    public void Reload()
    {
        
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
            hit.collider.gameObject.GetComponent<OnPhysicsEvent>()?.OnHitFunc(weaponData.Damage);
        }
        else if (hit.collider.CompareTag("Head"))
        {
            hit.collider.gameObject.GetComponentInParent<OnPhysicsEvent>()?.OnHitFunc(weaponData.Damage * 1.5f);
            sound.PlayHeadShotSound();
        }
    }
    
    private void CreateCartridge(int count = 0)
    {
        var creatCount = (count == 0) ? weaponData.Magazine + 10 : count;

        for (var i = 0; i < creatCount; ++i)
        {
            var cartridge = Instantiate(weaponData.Cartridge, cartridgeOutPos).GetComponent<Cartridge>();

            cartridge.ReturnAction = ReturnCartridge;
            cartridge.gameObject.SetActive(false);
            
            _shells.Enqueue(cartridge);
        }
    }

    private Cartridge GetCartridge()
    {
        if (_shells.Count <= 0)
        {
            CreateCartridge();
        }

        var shell = _shells.Dequeue();

        shell.gameObject.transform.SetPositionAndRotation(cartridgeOutPos.position,
            weaponData.Cartridge.transform.rotation);
        shell.gameObject.SetActive(true);

        return shell;
    }

    private void ReturnCartridge(Cartridge cartridge)
    {
        cartridge.gameObject.SetActive(false);
        _shells.Enqueue(cartridge);
    }
}
