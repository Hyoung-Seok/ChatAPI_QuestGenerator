using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : UnitStateController
{
    [Header("Stat")] 
    [SerializeField] private float MaxHP = 100.0f;

    [Header("Animation Hash")]
    public readonly int SpeedKey = Animator.StringToHash("Speed");
    private readonly int _idleAnimation = Animator.StringToHash("IdleMotions");
    private readonly int _playerHpKey = Animator.StringToHash("PlayerHP");
    private readonly int _quippedTriggerKey = Animator.StringToHash("Equipped");
    private readonly int _equippedKey = Animator.StringToHash("IsEquipped");
    private readonly int _aimKey = Animator.StringToHash("Aim");
    
    [Header("State And Data")]
    private readonly PlayerMoveState _moveState;
    public PlayerComponentData ComponentData { get; private set; }
    
    [HideInInspector] public EPlayerInputState CurInputState;
    
    private float _currentHp;
    private bool _isEquipped = false;
    
    public PlayerController(PlayerMoveData moveData, PlayerComponentData componentData)
    {
        _moveState = new PlayerMoveState(this, moveData);
        ComponentData = componentData;
        
        moveData.OnValueChangeAction -= OnMoveValueChangeEvent;
        moveData.OnValueChangeAction += OnMoveValueChangeEvent;
        
        _currentHp = MaxHP;
        ComponentData.Animator.SetFloat(_playerHpKey, _currentHp);

        _isEquipped = false;
        ComponentData.Animator.SetBool(_equippedKey, false);
        
        ChangeMainState(_moveState);
    }

    public void PlayerDamaged(float dmg)
    {
        _currentHp -= dmg;
        ComponentData.Animator.SetFloat(_playerHpKey, _currentHp);

        if (_currentHp <= 0)
        {
            // TODO : 사망처리
        }
    }

    public void ChangeIdleAnimation()
    {
        if (_currentHp < 30)
        {
            ComponentData.Animator.SetFloat(_idleAnimation, 4);
            return;
        }
        
        var num = Random.Range(1,4);
        ComponentData.Animator.SetFloat(_idleAnimation, num);
    }

    public void ChangePlayerInputState(EPlayerInputState inputState)
    {
        switch (inputState)
        {
            case EPlayerInputState.IDLE:
                ComponentData.WeaponManager.ChangeWeaponState(inputState);
                ComponentData.Animator.SetBool(_aimKey, false);
                
                _moveState.ChangeMoveSpeed(inputState, _isEquipped);
                break;
            
            case EPlayerInputState.WALK:
            case EPlayerInputState.RUN:
                _moveState.ChangeMoveSpeed(inputState, _isEquipped);
                break;
            
            case EPlayerInputState.EQUIPPED:
                EquippedWeapon();
                _moveState.ChangeMoveSpeed(inputState, _isEquipped);
                break;
            
            case EPlayerInputState.AIM:
                ComponentData.WeaponManager.ChangeWeaponState(inputState);
                ComponentData.Animator.SetBool(_aimKey, true);
                
                _moveState.ChangeMoveSpeed(inputState, _isEquipped);
                break;
            
            default:
                return;
        }
        
        CurInputState = inputState;
    }
    
    private void EquippedWeapon()
    {
        if (_isEquipped == true)
        {
            _isEquipped = false;
            ComponentData.Animator.SetBool(_equippedKey, _isEquipped);
            ComponentData.Animator.SetTrigger(_quippedTriggerKey);
            return;
        }

        _isEquipped = true;
        ComponentData.Animator.SetBool(_equippedKey, _isEquipped);
        ComponentData.Animator.SetTrigger(_quippedTriggerKey);
    }

    private void OnMoveValueChangeEvent(PlayerMoveData data)
    {
        _moveState.OnValueUpdate(data);
    }
}
