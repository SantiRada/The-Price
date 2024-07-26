using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionForControlPlayer : MonoBehaviour {

    [Header("Elements of Player")]
    private PlayerMovement _movement;
    private CrosshairData _crosshair;
    private PlayerInput _playerInput;
    private PlayerStats _playerStats;
    public List<WeaponSystem> _weapon = new List<WeaponSystem>();

    [Header("UI Content")]
    private StatsInUI _statsInUI;
    private InputSystemManager _inputSystem;

    [Header("Crosshair")]
    private bool aimWithStick = false;

    // EVENTOS
    public static event Action skillOne;
    public static event Action skillTwo;
    public static event Action skillFragments;

    public static bool detectClic = true;

    [Header("Timers")]
    private float delayPressIsUse = 0.12f;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerStats = GetComponent<PlayerStats>();
        _movement = GetComponent<PlayerMovement>();
        _statsInUI = FindAnyObjectByType<StatsInUI>();
        _crosshair = GetComponentInChildren<CrosshairData>();
        _inputSystem = FindAnyObjectByType<InputSystemManager>();
    }
    private void Start() { PlayerStats.changesInWeapons += ChangesWeapons; }
    private void Update()
    {
        DelayPerPressNotHold();

        if (!detectClic || Pause.Comprobation(State.Game)) return;

        _movement.SetDirection(_playerInput.actions["Move"].ReadValue<Vector2>());

        if (!aimWithStick) _crosshair.SetAimDirection(_playerInput.actions["Move"].ReadValue<Vector2>());
        else AimWithRightStick();
    }
    private void DelayPerPressNotHold()
    {
        if (PlayerActionStates.IsUse)
        {
            delayPressIsUse -= Time.deltaTime;

            if(delayPressIsUse <= 0)
            {
                delayPressIsUse = 0.12f;
                PlayerActionStates.IsUse = false;
            }
        }
        else { delayPressIsUse = 0.12f; }
    }
    public static void ChangeDetectClic(bool value) { detectClic = value; }
    // ----------------------------- //
    public void Dash(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Started)
        {
            if (Pause.state == State.Game)
            {
                PlayerActionStates.IsDashing = true;
                _inputSystem.ApplyAnimation("Dash");
            }
        }
        if (context.phase == InputActionPhase.Performed)
        {
            // ENVIAR A LA SIGUIENTE SALA AL CLIQUEAR ESTO SI ESTÁ MUERTO
            if (_playerStats.isDead) { _playerStats.deadSystem.DiePlayer(); }
            else { if (_movement.GetCanDashing() && _movement.GetCanMove()) { _movement.StartCoroutine("Roll"); } }
        }
        if (context.phase == InputActionPhase.Canceled) { if (Pause.state == State.Game) PlayerActionStates.IsDashing = false; }
    }
    // ----------------------------- //
    public void Attack(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsAttacking = true;
            _inputSystem.ApplyAnimation("AttackOne");
        }
        if(context.phase == InputActionPhase.Performed)
        {
            // ENVIAR A LA SIGUIENTE SALA AL CLIQUEAR ESTO SI ESTÁ MUERTO
            if (_playerStats.isDead) _playerStats.deadSystem.DiePlayer();

            if (_weapon.Count > 0) if(_weapon[0] != null) _weapon[0].PrepareAttack();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            PlayerActionStates.IsAttacking = false;
        }
    }
    public void AttackTwo(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Performed)
        {
            if (_weapon.Count > 1) if(_weapon[1] != null) _weapon[1].PrepareAttack();
        }
        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsAttackingTwo = true;
            _inputSystem.ApplyAnimation("AttackTwo");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsAttackingTwo = false;
    }
    public void AttackThree(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Performed)
        {
            if (_weapon.Count > 2) if (_weapon[2] != null) _weapon[2].PrepareAttack();
        }
        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsAttackingThree = true;
            _inputSystem.ApplyAnimation("AttackThree");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsAttackingThree = false;
    }
    // ----------------------------- //
    public void Use(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state == State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsUse = true;
            _inputSystem.ApplyAnimation("Use");
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
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsSkillOne = true;
            _inputSystem.ApplyAnimation("SkillOne");
        }
        if (context.phase == InputActionPhase.Performed) skillOne?.Invoke();
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsSkillOne = false;
    }
    public void SkillTwo(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsSkillTwo = true;
            _inputSystem.ApplyAnimation("SkillTwo");
        }

        if (context.phase == InputActionPhase.Performed)
        {
            if (Pause.state == State.Interface) _statsInUI.CloseUI();
            skillTwo?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsSkillTwo = false;
    }
    public void SkillThree(InputAction.CallbackContext context)
    {
        if (!detectClic) return;
        if (Pause.state != State.Game) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsSkillThree = true;
            _inputSystem.ApplyAnimation("SkillThree");
        }

        if (context.phase == InputActionPhase.Performed) skillFragments?.Invoke();

        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.IsSkillThree = false;
    }
    public void StaticAim(InputAction.CallbackContext context)
    {
        if (!detectClic) return;

        if (Pause.Comprobation(State.Game)) return;

        if (context.phase == InputActionPhase.Started)
        {
            aimWithStick = true;
            _inputSystem.ApplyAnimation("StaticAim");
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            aimWithStick = false;
        }
    }
    public void Stats()
    {
        _inputSystem.ApplyAnimation("Stats");
        if (Pause.state == State.Pause)
        {
            PlayerActionStates.InStats = false;
            _playerInput.SwitchCurrentActionMap("Player");
            _statsInUI.CloseUI();
        }
        else if(Pause.state == State.Game)
        {
            PlayerActionStates.InStats = true;
            _playerInput.SwitchCurrentActionMap("Pause");
            _statsInUI.ShowWindowedStats();
        }
    }
    public void PauseAction()
    {
        _inputSystem.ApplyAnimation("Pause");
        if (Pause.state == State.Game)
        {
            PlayerActionStates.inPause = true;
            _playerInput.SwitchCurrentActionMap("Pause");
            _statsInUI.ShowWindowedPause();
        }
        else if(Pause.state == State.Pause)
        {
            PlayerActionStates.inPause = false;
            _playerInput.SwitchCurrentActionMap("Player");
            _statsInUI.CloseUI();
        }
    }
    // ---- UI ACTIONS ------------- //
    public void SelectInUI(InputAction.CallbackContext context)
    {
        if (Pause.state != State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.selectUI = true;
            _inputSystem.ApplyAnimation("Select");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.selectUI = false;
    }
    public void BackInUI(InputAction.CallbackContext context)
    {
        if (Pause.state != State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.backUI = true;
            _inputSystem.ApplyAnimation("Back");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.backUI = false;
    }
    public void OtherFunctionInUI(InputAction.CallbackContext context)
    {
        if (Pause.state != State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.otherFunctionUI = true;
            _inputSystem.ApplyAnimation("OtherFunction");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.otherFunctionUI = false;
    }
    public void ResetValuesInUI(InputAction.CallbackContext context)
    {
        if (Pause.state != State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.resetValuesUI = true;
            _inputSystem.ApplyAnimation("ResetValues");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.resetValuesUI = false;
    }
    public void LeftInUI(InputAction.CallbackContext context)
    {
        if (Pause.state != State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.leftUI = true;
            _inputSystem.ApplyAnimation("LeftUI");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.leftUI = false;
    }
    public void RightInUI(InputAction.CallbackContext context)
    {
        if (Pause.state != State.Pause) return;

        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.rightUI = true;
            _inputSystem.ApplyAnimation("RightUI");
        }
        if (context.phase == InputActionPhase.Canceled) PlayerActionStates.rightUI = false;
    }
    // ----------------------------- //
    private void AimWithRightStick()
    {
        if (!detectClic) return;

        if (Pause.Comprobation(State.Game)) return;

        _crosshair.SetAimDirection(_playerInput.actions["Aim"].ReadValue<Vector2>());
    }
    public Vector2 RightStick() { return _playerInput.actions["Aim"].ReadValue<Vector2>(); }
    public static class PlayerActionStates
    {
        public static bool IsDashing { get; set; }
        public static bool IsAttacking { get; set; }
        public static bool IsAttackingTwo { get; set; }
        public static bool IsAttackingThree { get; set; }
        public static bool IsSkillOne { get; set; }
        public static bool IsSkillTwo { get; set; }
        public static bool IsSkillThree { get; set; }
        public static bool IsUse { get; set; }
        public static bool InStats { get; set; }

        // ---- UI ACTIONS ---- //
        public static bool backUI { get; set; } // B
        public static bool selectUI { get; set; } // A
        public static bool otherFunctionUI { get; set; } // X
        public static bool resetValuesUI { get; set; } // Y
        public static bool leftUI { get; set; } // LB
        public static bool rightUI { get; set; } // RB
        public static bool inPause {  get; set; } // Start
    }
    // ---- FUNCION INTEGRA X EVENTO ---- //
    private void ChangesWeapons() { _weapon = _playerStats.weaponInScene; }
}