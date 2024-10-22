using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : UnitStateController
{
    [HideInInspector] public EPlayerInputState CurInputState;
    
    // Component
    public Rigidbody PlayerRig { get; private set; }
    public Animator Animator { get; private set; }
    public Transform Transform { get; private set; }
    public Transform CameraDir { get; private set; }
    public AudioSource AudioSource { get; private set; }
    public bool IsEquipped { get; private set; }
    
    [Header("Status")] 
    private float _lv;
    public float PlayerMaxHp { get; private set; }
    
    [Header("Player State")]
    private readonly PlayerMoveState _moveState;
    private readonly PlayerDeathState _deathState;
    public PlayerMoveState PlayerMoveState => _moveState;
    
    private float _currentHp;
    private readonly AudioClip[] _hitClips;
    private readonly GameManager _instance;

    #region AnimationKey
    
    public readonly int SpeedKey = Animator.StringToHash("Speed");
    private readonly int _idleAnimation = Animator.StringToHash("IdleMotions");
    private readonly int _playerHpKey = Animator.StringToHash("PlayerHP");
    private readonly int _quippedTriggerKey = Animator.StringToHash("Equipped");
    private readonly int _equippedKey = Animator.StringToHash("IsEquipped");
    private readonly int _aimKey = Animator.StringToHash("Aim");
    private readonly int _deathKey = Animator.StringToHash("IsDead");
    private readonly int _reloading = Animator.StringToHash("Reloading");
    
    #endregion
    
    public PlayerController(PlayerStatus status, PlayerComponentData componentData)
    {
        PlayerRig = componentData.Rig;
        Animator = componentData.Animator;
        Transform = componentData.PlayerTransform;
        CameraDir = componentData.CameraDir;
        AudioSource = componentData.AudioSource;

        _currentHp = PlayerMaxHp = status.MaxHp;
        _lv = status.Level;
        
        status.OnValueChangeAction -= OnMoveValueChangeEvent;
        status.OnValueChangeAction += OnMoveValueChangeEvent;
            
        IsEquipped = false;
        Animator.SetFloat(_playerHpKey, _currentHp);
        Animator.SetBool(_equippedKey, false);

        _instance = GameManager.Instance;
        _hitClips = _instance.AudioManager.GetAudioClips("HitVoice");
        
        _moveState = new PlayerMoveState(this, status);
        _deathState = new PlayerDeathState(this);
        
        ChangeMainState(_moveState);
    }

    public void PlayerDamaged(float dmg)
    {
        _currentHp -= dmg;
        Debug.Log($"Player HP : {_currentHp}");
        
        Animator.SetFloat(_playerHpKey, _currentHp);
        
        PlayAudio(AudioSource, _hitClips[Random.Range(0, _hitClips.Length)]);
        _instance.AudioManager.PlaySound(ESoundType.EFFECT, "HitSound", true, Transform.position);
        _instance.CameraEffect.ShakeCamera(ECameraShake.HIT, 1.5f);
        
        if (_currentHp > 30)
        {
            _instance.StartHitCameraEffect();
        }
        else if (_currentHp <= 30 && _currentHp > 0)
        {
            _instance.StartHealthEffect();
        }
        else
        {
            Animator.SetBool(_deathKey, true);
            ChangeMainState(_deathState);
        }
    }

    public void ChangeIdleAnimation()
    {
        if (_currentHp < 30)
        {
            Animator.SetFloat(_idleAnimation, 4);
            return;
        }
        
        var num = Random.Range(1,4);
        Animator.SetFloat(_idleAnimation, num);
    }

    public void ChangePlayerInputState(EPlayerInputState inputState)
    {
        switch (inputState)
        {
            case EPlayerInputState.IDLE:
                GameManager.Instance.WeaponManager.ChangeWeaponState(inputState);
                Animator.SetBool(_aimKey, false);
                
                _moveState.ChangeMoveSpeed(inputState, IsEquipped);
                break;
            
            case EPlayerInputState.WALK:
            case EPlayerInputState.RUN:
                _moveState.ChangeMoveSpeed(inputState, IsEquipped);
                break;
            
            case EPlayerInputState.EQUIPPED:
                EquippedWeapon();
                _moveState.ChangeMoveSpeed(inputState, IsEquipped);
                break;
            
            case EPlayerInputState.AIM when (IsEquipped == true) : 
                GameManager.Instance.WeaponManager.ChangeWeaponState(inputState);
                Animator.SetBool(_aimKey, true);
                
                _moveState.ChangeMoveSpeed(inputState, IsEquipped);
                break;
            
            case EPlayerInputState.RELOADING 
                when GameManager.Instance.WeaponManager.IsReloading == false &&
                     GameManager.Instance.WeaponManager.CanReloading == true :
                Animator.SetTrigger(_reloading);
                break;
            
            default:
                return;
        }
        
        CurInputState = inputState;
    }

    public void ResetPlayer()
    {
        Animator.SetBool(_deathKey, false);
        _currentHp = PlayerMaxHp;
        
        if (IsEquipped == true)
        {
            ChangePlayerInputState(EPlayerInputState.EQUIPPED);   
        }
        
        Transform.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.identity);
        SpawnerManager.Instance.ResetAllEnemyTargetTransform(Transform);
        
        ChangeMainState(_moveState);
    }
    
    private void PlayAudio(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
    
    private void EquippedWeapon()
    {
        if (IsEquipped == true)
        {
            IsEquipped = false;
            Animator.SetBool(_equippedKey, IsEquipped);
            Animator.SetTrigger(_quippedTriggerKey);
            GameManager.Instance.UIManager.SetActiveMagazineUI(false);
            return;
        }

        IsEquipped = true;
        Animator.SetBool(_equippedKey, IsEquipped);
        Animator.SetTrigger(_quippedTriggerKey);
        GameManager.Instance.UIManager.SetActiveMagazineUI(true);
    }

    private void OnMoveValueChangeEvent(PlayerStatus data)
    {
        _moveState.OnValueUpdate(data);
    }
}
