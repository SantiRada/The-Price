using System.Collections;
using UnityEngine;

public enum TypeEnemyAttack { Base, Energy, Fire, Cold, Fortify }
public abstract class EnemyManager : MonoBehaviour {

    [Header("UI Content")]
    public GameObject uiObject;
    public Vector2 offsetPositionUI;
    private Canvas _worldPosition;
    private EnemyUI _enemyUI;

    [Header("Initial Values")]
    public int _weight;
    [Range(0, 100)] public int _probabilityOfAppearing;

    [Header("Attack Enemy")]
    public TypeEnemyAttack typeAttack;
    public bool distanceAttack;
    [Tooltip("Velocidad del Proyectil")] public float speedAttack;
    [Tooltip("Distancia necesaria para atacar")] public float rangeAttack;
    [Tooltip("Distancia del proyectil antes de ser destruido")] public float distanceToAttack;
    [Tooltip("Tiempo entre ataques")] public float delayBetweenAttack;
    protected bool _canAttack { get; set; }

    [Header("Stats")]
    public float healthMax;
    public float shieldMax;
    public int damage;
    [SerializeField, Tooltip("Valor Promedio: 1.5"), Range(0f, 6f)] private float _speed;
    [HideInInspector] public float shield;
    protected float health;
    protected bool _canMove { get; set; }

    [Header("Jump Data")]
    public bool canJump = true;
    [SerializeField, Tooltip("Distancia que debe haber para saltar, Valor Promedio: 4"), Range(1, 8)] private int _distanceToJump;
    [SerializeField, Tooltip("Que tan lejos del Player quedo tras el salto, Valor Promedio: 4"), Range(3, 8)] private int _howFarDoJump;
    [SerializeField, Tooltip("Tiempo que tarda en permitirse el salto nuevamente"), Range(0, 5)] private float _delayToJump;
    [HideInInspector] public bool inJump = false;

    [Header("Private Data")]
    protected Rigidbody2D _rb2d;
    protected SpriteRenderer _spr;
    protected PlayerStats _player;
    protected Animator _anim;
    private Room _room { get; set; }
    [Space]
    private float delayBase;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        _player = FindAnyObjectByType<PlayerStats>();
        _worldPosition = FindAnyObjectByType<InteractiveManager>().GetComponent<Canvas>();

        // ESTABLECER VALORES INICIALES
        health = healthMax;
        shield = shieldMax;

        // CREAR UI PARA CADA ENEMIGO
        _enemyUI = Instantiate(uiObject, ((Vector2)transform.position + offsetPositionUI), Quaternion.identity, _worldPosition.transform).GetComponent<EnemyUI>();
        _enemyUI.SetInitialValues(gameObject, offsetPositionUI);

        StartCoroutine("DelayToMovement");

        delayBase = delayBetweenAttack;
    }
    private void Update()
    {
        if (CanMove)
        {
            if (_player.transform.position.x > transform.position.x) _spr.flipX = true;
            else _spr.flipX = false;

            if(_player.transform.position.y > transform.position.y) _anim.SetBool("Direction", false);
            else _anim.SetBool("Direction", true);
        }

        if (Vector3.Distance(_player.transform.position, transform.position) <= rangeAttack)
        {
            VerifyState(false);
            if (CanAttack) Attack();
        }
        else { VerifyState(true); }

        if (!CanAttack)
        {
            delayBetweenAttack -= Time.deltaTime;

            if (delayBetweenAttack <= 0)
            {
                delayBetweenAttack = delayBase;
                CanAttack = true;
            }
        }

        if (health <= 0) Die();
    }
    public void TakeDamage(int dmg)
    {
        if (shield > 0)
        {
            if (shield > dmg) shield -= dmg;
            else shield = 0;
        }
        else { health -= dmg; }
        _enemyUI.SetHealthbar(healthMax, health, shieldMax, shield);

        FloatTextManager.CreateText(transform.position, TypeColor.Damage, "-" + dmg.ToString());
    }
    public abstract void Attack();
    public void Die()
    {
        _room?.SetLivingEnemies(this);
        
        Destroy(_enemyUI.gameObject);
        Destroy(gameObject);
    }
    public IEnumerator DelayToMovement()
    {
        CanMove = false;
        yield return new WaitForSeconds(1.5f);
        CanMove = true;
        CanAttack = true;
    }
    public IEnumerator DelayToJump()
    {
        canJump = false;
        inJump = false;
        yield return new WaitForSeconds(_delayToJump);
        canJump = true;
    }
    // ---- MODIFICATORS ---- //
    public void AddState(TypeState state, int numberOfLoads)
    {
        if(state == TypeState.Stun && numberOfLoads == 2)
        {
            if (Weight > 1) numberOfLoads = 1;
        }

        AffectedState st = gameObject.AddComponent<AffectedState>();
        st.CreateState(state, numberOfLoads);
    }
    public void VerifyState(bool value)
    {
        if (GetComponent<AffectedState>()) return;
        else CanMove = value;
    }
    // ---- SETTERS && GETTERS ---- //
    public Room RoomCurrent { set { _room = value; } }
    public int ProbabilityOfAppearing { get { return _probabilityOfAppearing; } }
    public int DistanceToJump { get { return _distanceToJump; } }
    // ---- SETTERS && GETTERS PER STATS ---- //
    public int Weight { get { return _weight; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public int Shield { get { return (int)shield; } set { shield = value; } }
    // ---- SETTERS && GETTERS PER BOOLEAN ---- //
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    public bool CanAttack { get { return _canAttack; } set { _canAttack = value; } }
}