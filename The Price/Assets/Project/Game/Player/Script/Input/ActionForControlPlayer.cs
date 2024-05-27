using UnityEngine.InputSystem;
using UnityEngine;

public class ActionForControlPlayer : MonoBehaviour {

    [Header("Elements of Player")]
    private CrosshairData _crosshair;
    private PlayerMovement _movement;
    private PlayerInput _playerInput;

    [Header("Crosshair")]
    private bool aimWithStick = false;

    [Header("Stats")]
    private float timer = 0.3f;
    private bool canChangeStats = true;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
        _crosshair = GetComponentInChildren<CrosshairData>();
    }
    private void Update()
    {
        if (Pause.inPause) return;

        _movement.SetDirection(_playerInput.actions["Move"].ReadValue<Vector2>());

        if (!aimWithStick) _crosshair.SetAimDirection(_playerInput.actions["Move"].ReadValue<Vector2>());
        else AimWithRightStick();

        // ---- SELECT ---- //
        if (!canChangeStats) timer -= Time.deltaTime;

        if (!canChangeStats && timer <= 0)
        {
            canChangeStats = true;
            timer = 0.3f;
        }
        // ---------------- //
    }
    public void Dash()
    {
        if (_movement.GetCanDashing() && _movement.GetCanMove()) _movement.StartCoroutine("Roll");
    }
    public void StaticAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) aimWithStick = true;
        else if (context.phase == InputActionPhase.Canceled) aimWithStick = false;
    }
    private void AimWithRightStick()
    {
        _crosshair.SetAimDirection(_playerInput.actions["Aim"].ReadValue<Vector2>());
    }
}