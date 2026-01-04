using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static ActionForControlPlayer;

public class PlayerMovement : MonoBehaviour {

    [Header("Movement")]
    [HideInInspector] public bool _canMove = true;

    [Header("Roll")]
    [SerializeField] private float _dashingPower = 24f;
    [SerializeField] private float _dashingTime = 0.2f;
    [SerializeField] private float _dashingCooldown = 1f;
    [HideInInspector] public bool isDashing = false;
    private bool _canDash = true;

    [Header("Private Data")]
    private Vector2 _moveInput = Vector2.zero;

    [Header("Element")]
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    [HideInInspector] public PlayerStats _playerStats;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _playerStats = GetComponent<PlayerStats>();
    }
    private void Update()
    {
        if (!Pause.IsGamePlaying()) return;

        Movement();
    }
    private void FixedUpdate()
    {
        if (isDashing) return;

        if (_canMove && Pause.IsGamePlaying()) _rigidbody2D.MovePosition(_rigidbody2D.position + _moveInput * (int)_playerStats.GetterStats(2, false) * Time.fixedDeltaTime);
        else _rigidbody2D.velocity = Vector2.zero;
    }
    private void Movement()
    {
        if (!_canMove) return;

        if(_rigidbody2D.velocity != Vector2.zero) _playerStats.JumpBetweenAttack();

        #region Flip
        if (_moveInput.x > 0) _spriteRenderer.flipX = true;
        else if (_moveInput.x < 0) _spriteRenderer.flipX = false;
        #endregion
    }
    public IEnumerator Roll()
    {
        // NO PUEDE RECIBIR DA�O DURANTE EL DASH
        _playerStats._canReceivedDamage = false;

        _playerStats.JumpBetweenAttack();
        _canDash = false;
        isDashing = true;

        _rigidbody2D.velocity = new Vector2(_moveInput.x, _moveInput.y).normalized * _dashingPower;

        yield return new WaitForSeconds(_dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
        PlayerActionStates.IsDashing = false;

        // PUEDE RECIBIR DA�O NUEVAMENTE
        _playerStats._canReceivedDamage = true;
    }
    // ---- SETTERS & GETTERS ---- //
    public void SetDirection(Vector2 values)
    {
        _moveInput = values;
    }
    public bool GetCanMove()
    {
        return _canMove;
    }
    public void SetMove(bool value)
    {
        if (Pause.state != State.Game) return;

        _canMove = value;
    }
    public bool GetCanDashing()
    {
        return _canDash;
    }
}