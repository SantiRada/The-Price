using System.Collections;
using UnityEngine;
using static ActionForControlPlayer;

public class PlayerMovement : MonoBehaviour {

    [Header("Movement")]
    private bool _canMove = true;

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
    private PlayerStats _player;

    private Vector2 direction;
    private bool canMoveForced = false;
    private float timerForced = 1.5f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _player = GetComponent<PlayerStats>();

        timerForced = 1.5f;
    }
    private void Update()
    {
        if (Pause.inPause || Pause.state != State.Game) return;

        Movement();

        if (canMoveForced)
        {
            Debug.Log("Forced Movement!");
            _rigidbody2D.AddForce(direction * 50 * Time.deltaTime, ForceMode2D.Impulse);
            timerForced -= Time.deltaTime;

            if (timerForced <= 0) canMoveForced = false;
        }
    }
    private void FixedUpdate()
    {
        if (isDashing) return;

        if (_canMove && !Pause.inPause && Pause.state == State.Game) _rigidbody2D.MovePosition(_rigidbody2D.position + _moveInput * (int)_player.GetterStats(2, false) * Time.fixedDeltaTime);
        else _rigidbody2D.velocity = Vector2.zero;
    }
    private void Movement()
    {
        if (!_canMove) return;

        #region Flip
        if (_moveInput.x > 0) _spriteRenderer.flipX = true;
        else if (_moveInput.x < 0) _spriteRenderer.flipX = false;
        #endregion
    }
    public IEnumerator Roll()
    {
        // NO PUEDE RECIBIR DAÑO DURANTE EL DASH
        _player.CanReceivedDamage = false;

        _player.JumpBetweenAttack();
        _canDash = false;
        isDashing = true;

        _rigidbody2D.velocity = new Vector2(_moveInput.x, _moveInput.y).normalized * _dashingPower;

        yield return new WaitForSeconds(_dashingTime);
        isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
        PlayerActionStates.IsDashing = false;

        // PUEDE RECIBIR DAÑO NUEVAMENTE
        _player.CanReceivedDamage = true;
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
    public bool GetCanDashing()
    {
        return _canDash;
    }
    // ---- TRIGGERS && COLLISIONS ---- //
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            _player.TakeDamage(collision.GetComponentInParent<EnemyManager>().gameObject, collision.GetComponentInParent<EnemyManager>().damage);
        }
    }
    public void SetValuesForcedMove(GameObject collision)
    {
        direction = -(collision.GetComponentInParent<EnemyManager>().transform.position - transform.position).normalized * 1.5f;
        canMoveForced = true;
        timerForced = 1.5f;
    }
}