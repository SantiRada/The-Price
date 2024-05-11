using UnityEngine.InputSystem;
using UnityEngine;

public class ActionForControlPlayer : MonoBehaviour {

    [Header("Elements of Player")]
    private CrosshairData _crosshair;
    private PlayerStats _playerStats;
    private WeaponSystem _weaponSystem;
    private PlayerMovement _movement;
    private PlayerInput _playerInput;

    [Header("Other Elements")]
    private PauseMenu _pauseMenu;

    [Header("Crosshair")]
    private bool aimWithStick = false;

    [Header("Stats")]
    private float timer = 0.3f;
    private bool canChangeStats = true;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerStats = GetComponent<PlayerStats>();
        _movement = GetComponent<PlayerMovement>();
        _crosshair = GetComponentInChildren<CrosshairData>();
        _weaponSystem = GetComponentInChildren<WeaponSystem>();

        _pauseMenu = FindAnyObjectByType<PauseMenu>();
    }
    private void Start()
    {
        if (!_playerStats.Control.Contains("Joystick")) _playerInput.defaultControlScheme = "Keyboard";
        else if (_playerStats.Control.Contains("Joystick"))  _playerInput.defaultControlScheme = "Gamepad";
    }
    private void Update()
    {
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
    public void Attack()
    {
        _weaponSystem.CallAttack();
    }
    public void ChangeAppearanceStats()
    {
        // ---- ESTABLECE EL VALOR DE "STATS" EN EL VALOR CONTRARIO AL ACTUAL --------- //
        if (canChangeStats)
        {
            _playerStats._stats.InStats = !_playerStats._stats.InStats;
            _playerStats._stats.gameObject.SetActive(_playerStats._stats.InStats);
            canChangeStats = false;
        }
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
    public void Pause()
    {
        _pauseMenu.Pause = !_pauseMenu.Pause;
    }
}