using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionForControlPlayer : MonoBehaviour {

    [Header("Elements of Player")]
    private PlayerMovement _movement;
    private CrosshairData _crosshair;
    private PlayerInput _playerInput;
    private PlayerStats _playerStats;
    private WeaponSystem _weapon;

    [Header("UI Content")]
    private StatsInUI _statsInUI;

    [Header("Crosshair")]
    private bool aimWithStick = false;

    // EVENTOS
    public static event Action skillOne;
    public static event Action skillTwo;
    public static event Action skillFragments;

    public static bool detectClic = true;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerStats = GetComponent<PlayerStats>();
        _movement = GetComponent<PlayerMovement>();
        _statsInUI = FindAnyObjectByType<StatsInUI>();
        _crosshair = GetComponentInChildren<CrosshairData>();
    }
    private void Start() { _weapon = GetComponentInChildren<WeaponSystem>(); }
    private void Update()
    {
        if (!detectClic || Pause.Comprobation(State.Game)) return;

        _movement.SetDirection(_playerInput.actions["Move"].ReadValue<Vector2>());

        if (!aimWithStick) _crosshair.SetAimDirection(_playerInput.actions["Move"].ReadValue<Vector2>());
        else AimWithRightStick();
    }
    public static void ChangeDetectClic(bool value) { detectClic = value; }
    // ----------------------------- //
    public void Dash()
    {
        // ENVIAR A LA SIGUIENTE SALA AL CLIQUEAR ESTO SI ESTÁ MUERTO
        if (_playerStats.isDead) _playerStats.deadSystem.DiePlayer();

        if (!detectClic) return;

        if (Pause.Comprobation(State.Game)) return;

        if (_movement.GetCanDashing() && _movement.GetCanMove())
        {
            _movement.StartCoroutine("Roll");
            PlayerActionStates.IsDashing = true;
        }
    }
    // ----------------------------- //
    public void Attack(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if(context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsAttacking = true;
        }
        if(context.phase == InputActionPhase.Performed)
        {
            // ENVIAR A LA SIGUIENTE SALA AL CLIQUEAR ESTO SI ESTÁ MUERTO
            if (_playerStats.isDead) _playerStats.deadSystem.DiePlayer();

            if (_weapon != null) _weapon.Attack();
            else Debug.Log("El PJ no tiene Arma");
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            PlayerActionStates.IsAttacking = false;
        }
    }
    public void AttackTwo(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (context.phase == InputActionPhase.Performed)
        {
            if (_weapon != null) _weapon.FinalAttack(1);
            else Debug.Log("El PJ no tiene Arma");
        }
        if (context.phase == InputActionPhase.Started) PlayerActionStates.IsAttackingTwo = true;
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsAttackingTwo = false;
    }
    public void AttackThree(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (context.phase == InputActionPhase.Performed)
        {
            if (_weapon != null) _weapon.FinalAttack(2);
            else Debug.Log("El PJ no tiene Arma");
        }
        if (context.phase == InputActionPhase.Started) PlayerActionStates.IsAttackingThree = true;
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsAttackingThree = false;
    }
    // ----------------------------- //
    public void Use(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsUse = true;
        }
        if(context.phase == InputActionPhase.Performed)
        {
            // ENVIAR A LA SIGUIENTE SALA AL CLIQUEAR ESTO SI ESTÁ MUERTO
            if (_playerStats.isDead) _playerStats.deadSystem.DiePlayer();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            PlayerActionStates.IsUse = false;
        }
    }
    public void SkillOne(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (context.phase == InputActionPhase.Started) PlayerActionStates.IsSkillOne = true;

        if (context.phase == InputActionPhase.Performed) skillOne?.Invoke();

        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsSkillOne = false;
    }
    public void SkillTwo(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (context.phase == InputActionPhase.Started) PlayerActionStates.IsSkillTwo = true;

        if (context.phase == InputActionPhase.Performed)
        {
            if (Pause.state == State.Interface) _statsInUI.CloseUI();
            skillTwo?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsSkillTwo = false;
    }
    public void SkillFragments(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (context.phase == InputActionPhase.Started) PlayerActionStates.IsSkillFragments = true;

        if (context.phase == InputActionPhase.Performed) skillFragments?.Invoke();

        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsSkillFragments = false;
    }
    public void StaticAim(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (Pause.Comprobation(State.Game)) return;

        if (context.phase == InputActionPhase.Started)
        {
            aimWithStick = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            aimWithStick = false;
        }
    }
    public void Stats()
    {
        if (!detectClic) return;

        if(Pause.state == State.Interface)
        {
            _statsInUI.CloseUI();
        }
        else
        {
            PlayerActionStates.InStats = true;
            _statsInUI.ShowWindowedStats();
        }
    }
    public void PauseAction()
    {
        if (!detectClic) return;

        if(Pause.state == State.Game)
        {
            PlayerActionStates.inPause = true;
            _statsInUI.ShowWindowedPause();
        }
        else
        {
            PlayerActionStates.inPause = false;
            _statsInUI.CloseUI();
        }
    }
    // ---- UI ACTIONS ------------- //
    public void LeftInUI(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started) PlayerActionStates.leftUI = true;
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.leftUI = false;
    }
    public void RightInUI(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) PlayerActionStates.rightUI = true; 
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.rightUI = false;
    }
    // ----------------------------- //
    private void AimWithRightStick()
    {
        if (!detectClic) return;

        if (Pause.Comprobation(State.Game)) return;

        _crosshair.SetAimDirection(_playerInput.actions["Aim"].ReadValue<Vector2>());
    }
    public static class PlayerActionStates
    {
        public static bool IsMeditate { get; set; }
        public static bool IsDashing { get; set; }
        public static bool IsAttacking { get; set; }
        public static bool IsAttackingTwo { get; set; }
        public static bool IsAttackingThree { get; set; }
        public static bool IsSkillOne { get; set; }
        public static bool IsSkillTwo { get; set; }
        public static bool IsSkillFragments { get; set; }
        public static bool IsUse { get; set; }
        public static bool InStats { get; set; }

        // ---- UI ACTIONS ---- //
        public static bool leftUI { get; set; }
        public static bool rightUI { get; set; }
        public static bool inPause {  get; set; }
    }
}