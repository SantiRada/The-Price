using System.Collections;
using UnityEngine;

public enum TypeEnemyAttack { Base, Energy, Fire, Cold, Fortify }
public abstract class EnemyManager : MonoBehaviour {

    [Header("Initial Values")]
    public int _weight;
    [Range(0, 100)] public int _probabilityOfAppearing;

    [Header("Attack Enemy")]
    public TypeEnemyAttack typeAttack;
    protected bool _canAttack { get; set; }

    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private int shield;
    [SerializeField, Tooltip("Valor Promedio: 1.5"), Range(0f, 6f)] private float _speed;
    private bool _canMove { get; set; }

    [Header("Jump Data")]
    public bool canJump = true;
    [SerializeField, Tooltip("Distancia que debe haber para saltar, Valor Promedio: 4"), Range(1, 8)] private int _distanceToJump;
    [SerializeField, Tooltip("Que tan lejos del Player quedo tras el salto, Valor Promedio: 4"), Range(3, 8)] private int _howFarDoJump;
    [SerializeField, Tooltip("Tiempo que tarda en permitirse el salto nuevamente"), Range(0, 5)] private float _delayToJump;
    [HideInInspector] public bool inJump = false;

    [Header("Private Content")]
    protected Rigidbody2D _rb2d;
    protected SpriteRenderer _spr;
    private Room _room { get; set; }

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        StartCoroutine("DelayToMovement");
    }
    private void Update()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game || !_canMove)
        {
            _rb2d.velocity = Vector2.zero;
            return;
        }
    }
    public void TakeDamage(int dmg) { health -= dmg; }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Pause.state == State.Game) Die();
    }
    public void Die()
    {
        _room?.SetLivingEnemies(this);
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
    // ---- SETTERS && GETTERS ---- //
    public Room RoomCurrent { set { _room = value; } }
    public int ProbabilityOfAppearing { get { return _probabilityOfAppearing; } }
    public int DistanceToJump { get { return _distanceToJump; } }
    public int HowFarDoJump { get { return _howFarDoJump; } }
    // ---- SETTERS && GETTERS PER STATS ---- //
    public int Weight { get { return _weight; } }
    public float Speed { get { return _speed; } }
    public int Health { get { return health; } }
    public int Shield { get { return shield; } set { shield = value; } }
    // ---- SETTERS && GETTERS PER BOOLEAN ---- //
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    public bool CanAttack { get { return _canAttack; } set { _canAttack = value; } }
}