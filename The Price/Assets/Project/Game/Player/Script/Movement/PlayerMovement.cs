using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Movement")]
    private bool _canMove = true;

    [Header("Roll")]
    [SerializeField] private float _dashingPower = 24f;
    [SerializeField] private float _dashingTime = 0.2f;
    [SerializeField] private float _dashingCooldown = 1f;
    private bool isDashing = false;
    private bool _canDash = true;

    [Header("Private Data")]
    private Vector2 _moveInput = Vector2.zero;

    [Header("Element")]
    private Rigidbody2D _rigidbody2D;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _spriteRenderer;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        Movement();
    }
    private void FixedUpdate()
    {
        if (isDashing) return;

        VerifyMoveForCamera();
        if (_canMove) _rigidbody2D.MovePosition(_rigidbody2D.position + _moveInput * _playerStats.Speed * Time.fixedDeltaTime);
        else _rigidbody2D.velocity = Vector2.zero;
    }
    private void Movement()
    {
        if (!_canMove) return;

        _playerStats.SetAnimatorFloat("xInput", _moveInput.x);
        _playerStats.SetAnimatorFloat("yInput", _moveInput.y);
        if(_moveInput.x != 0 || _moveInput.y != 0) _playerStats.SetAnimatorBool("Move", true);
        else _playerStats.SetAnimatorBool("Move", false);

        #region Flip
        if (_moveInput.x > 0) _spriteRenderer.flipX = true;
        else if (_moveInput.x < 0) _spriteRenderer.flipX = false;
        #endregion
    }
    public IEnumerator Roll()
    {
        _canDash = false;
        isDashing = true;

        _rigidbody2D.velocity = new Vector2(_moveInput.x, _moveInput.y).normalized * _dashingPower;

        _trailRenderer.emitting = true;
        _playerStats.SetAnimatorBool("Dash", true);
        yield return new WaitForSeconds(_dashingTime);
        _playerStats.SetAnimatorBool("Dash", false);
        _trailRenderer.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
    }
    private void VerifyMoveForCamera()
    {
        Vector3 distance = Vector3.zero;
        // ------------------------------ //
        if (transform.position.x > CameraMovement.GetPosition().x) distance = transform.position - CameraMovement.GetPosition();
        else distance = CameraMovement.GetPosition() - transform.position;
        // ------------------------------ //
        if (distance.x >= 7.5f)
        {
            if(transform.position.x > CameraMovement.GetPosition().x) _moveInput.x = _moveInput.x > 0 ? 0 : _moveInput.x;
            else _moveInput.x = _moveInput.x > 0 ? _moveInput.x : 0;
        }
        if(Mathf.Abs(distance.y) >= 4f)
        {
            if(transform.position.y < CameraMovement.GetPosition().y) _moveInput.y = _moveInput.y < 0 ? 0 : _moveInput.y;
            else _moveInput.y = _moveInput.y > 0 ? 0 : _moveInput.y;
        }
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
}