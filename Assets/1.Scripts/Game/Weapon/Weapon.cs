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
    
    private Queue<Cartridge> _shells;
    
    private bool _canFire = true;
    private float _curTime = 0;

    private void Start()
    {
        Init();
    }

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
        
        if (Input.GetButton("Fire1") == true && _canFire == true)
        {
            var cartridge = GetCartridge();
            cartridge.StartCellDischarge(cartridgeOutPos.forward);
            
            muzzleEffect.Play();
            sound.PlayFireSound();
            
            _canFire = false;
            _curTime = 0;
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
