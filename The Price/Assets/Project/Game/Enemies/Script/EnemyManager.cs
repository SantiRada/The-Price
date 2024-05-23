using UnityEngine;

public abstract class EnemyManager : MonoBehaviour {

    [Header("Initial Values")]
    [SerializeField] private int _weight;
    [SerializeField, Range(0, 100)] private int _probabilityOfAppearing;
    [SerializeField] private float _speed;
    private bool _canMove { get; set; }

    [Header("Stats")]
    [SerializeField] private float speed;

    [Header("Private Content")]
    protected Rigidbody2D _rb2d;
    protected SpriteRenderer _spr;
    private Room _room { get; set; }

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (LoadingScreen.inLoading || Pause._inPause || !_canMove)
        {
            _rb2d.velocity = Vector2.zero;
            return;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Die();
    }
    public void Die()
    {
        _room?.SetLivingEnemies(this);
        Destroy(gameObject);
    }
    // ---- SETTERS && GETTERS ---- //
    public Room RoomCurrent { set { _room = value; } }
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    public int Weight { get { return _weight; } }
    public int ProbabilityOfAppearing { get { return _probabilityOfAppearing; } }
    public float Speed { get { return _speed; } }
}