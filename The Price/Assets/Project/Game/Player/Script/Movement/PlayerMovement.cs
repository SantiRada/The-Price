using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private float _speed;
    private bool _canMove = true;

    [Header("Roll")]
    [SerializeField] private float _dashingPower = 24f;
    [SerializeField] private float _dashingTime = 0.2f;
    [SerializeField] private float _dashingCooldown = 1f;
    [HideInInspector] public bool isDashing = false;
    private bool _canDash = true;

    [Header("Skills")]
    public List<SkillManager> skills = new List<SkillManager>();

    [Header("Private Data")]
    private Vector2 _moveInput = Vector2.zero;

    [Header("Element")]
    private Rigidbody2D _rigidbody2D;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (Pause.inPause) return;

        Movement();
    }
    private void FixedUpdate()
    {
        if (isDashing) return;

        if (_canMove && !Pause.inPause) _rigidbody2D.MovePosition(_rigidbody2D.position + _moveInput * _speed * Time.fixedDeltaTime);
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
        _canDash = false;
        isDashing = true;

        _rigidbody2D.velocity = new Vector2(_moveInput.x, _moveInput.y).normalized * _dashingPower;

        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        _trailRenderer.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
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