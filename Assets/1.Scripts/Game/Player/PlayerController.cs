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
    public bool IsEquipped { get; private set; }
    
    [Header("Status")] 
    private float _lv;
    public float PlayerMaxHp { get; private set; }
    
    [Header("Player State")]
    private readonly PlayerMoveState _moveState;
    
    private float _currentHp;

    #region AnimationKey
    
    [Header("Animation Hash")]
    public readonly int SpeedKey = Animator.StringToHash("Speed");
    private readonly int _idleAnimation = Animator.StringToHash("IdleMotions");
    private readonly int _playerHpKey = Animator.StringToHash("PlayerHP");
    private readonly int _quippedTriggerKey = Animator.StringToHash("Equipped");
    private readonly int _equippedKey = Animator.StringToHash("IsEquipped");
    private readonly int _aimKey = Animator.StringToHash("Aim");
    
    #endregion
    
    public PlayerController(PlayerStatus status, PlayerComponentData componentData)
    {
        PlayerRig = componentData.Rig;
        Animator = componentData.Animator;
        Transform = componentData.PlayerTransform;
        CameraDir = componentData.CameraDir;

        _currentHp = PlayerMaxHp = status.MaxHp;
        _lv = status.Level;
        
        status.OnValueChangeAction -= OnMoveValueChangeEvent;
        status.OnValueChangeAction += OnMoveValueChangeEvent;
            
        IsEquipped = false;
        Animator.SetFloat(_playerHpKey, _currentHp);
        Animator.SetBool(_equippedKey, false);
        
        _moveState = new PlayerMoveState(this, status);
        ChangeMainState(_moveState);
    }

    public void PlayerDamaged(float dmg)
    {
        _currentHp -= dmg;
        Debug.Log($"Player HP : {_currentHp}");
        Animator.SetFloat(_playerHpKey, _currentHp);

        if (_currentHp <= 0)
        {
            // TODO : 사망처리
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
            
            default:
                return;
        }
        
        CurInputState = inputState;
    }
    
    private void EquippedWeapon()
    {
        if (IsEquipped == true)
        {
            IsEquipped = false;
            Animator.SetBool(_equippedKey, IsEquipped);
            Animator.SetTrigger(_quippedTriggerKey);
            return;
        }

        IsEquipped = true;
        Animator.SetBool(_equippedKey, IsEquipped);
        Animator.SetTrigger(_quippedTriggerKey);
    }

    private void OnMoveValueChangeEvent(PlayerStatus data)
    {
        _moveState.OnValueUpdate(data);
    }
}
