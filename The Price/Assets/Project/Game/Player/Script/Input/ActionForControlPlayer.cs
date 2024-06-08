using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionForControlPlayer : MonoBehaviour {

    [Header("Elements of Player")]
    private CrosshairData _crosshair;
    private PlayerMovement _movement;
    private PlayerStats _stats;
    private PlayerInput _playerInput;

    [Header("Crosshair")]
    private bool aimWithStick = false;

    // EVENTOS
    public static event Action skillOne;
    public static event Action skillTwo;
    public static event Action skillFragments;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
        _crosshair = GetComponentInChildren<CrosshairData>();
    }
    private void Update()
    {
        if (Pause.Comprobation(State.Game)) return;

        _movement.SetDirection(_playerInput.actions["Move"].ReadValue<Vector2>());

        if (!aimWithStick) _crosshair.SetAimDirection(_playerInput.actions["Move"].ReadValue<Vector2>());
        else AimWithRightStick();
    }
    private void CloseStats()
    {
        if (PlayerActionStates.InStats)
        {
            _stats.ShowWindowedStats();
            PlayerActionStates.InStats = false;
        }
    }
    // ----------------------------- //
    public void Dash()
    {
        if (Pause.Comprobation(State.Game)) return;

        if (_movement.GetCanDashing() && _movement.GetCanMove())
        {
            _movement.StartCoroutine("Roll");
            PlayerActionStates.IsDashing = true;
        }
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsAttacking = true;
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            PlayerActionStates.IsAttacking = false;
        }
    }
    public void Use(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            PlayerActionStates.IsUse = true;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            PlayerActionStates.IsUse = false;
        }
        if(context.phase == InputActionPhase.Disabled)
        {
            PlayerActionStates.IsUse = false;
        }
    }
    public void SkillOne()
    {
        skillOne?.Invoke();
    }
    public void SkillTwo()
    {
        skillTwo?.Invoke();
    }
    public void SkillFragments()
    {
        skillFragments?.Invoke();
    }
    public void StaticAim(InputAction.CallbackContext context)
    {
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
    public void Stats(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PlayerActionStates.InStats = true;
            _stats.ShowWindowedStats();
        }
    }
    public void PauseAction(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            if (Pause.state == State.Game) Pause.StateChange = State.Interface;

            CloseStats();
        }
    }
    private void AimWithRightStick()
    {
        if (Pause.Comprobation(State.Game)) return;

        _crosshair.SetAimDirection(_playerInput.actions["Aim"].ReadValue<Vector2>());
    }
    public static class PlayerActionStates
    {
        public static bool IsDashing { get; set; }
        public static bool IsAttacking { get; set; }
        public static bool IsSkillOne { get; set; }
        public static bool IsSkillTwo { get; set; }
        public static bool IsSkillFragments { get; set; }
        public static bool IsUse { get; set; }
        public static bool InStats { get; set; }
    }
}