using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Data Projectile")]
    public float speedMovement;
    public GameObject spread;
    [HideInInspector] public int damage;
    [HideInInspector] public int whoIsBoss = 0;
    [HideInInspector] public bool canTraverse = false;
    [HideInInspector] public bool canAreaDamage = false;
    [Space]
    private GameObject gameObj;
    private float _distanceToAttack;

    private Vector2 _target;
    private Vector2 _initPos;
    private Rigidbody2D _rb2d;
    private Collider2D _collider;

    // Object Pooling
    private ProjectilePool _pool;
    private GameObject _prefab;
    private bool _usePooling = false;

    private void OnEnable()
    {
        _collider = GetComponent<Collider2D>();
        _rb2d = GetComponent<Rigidbody2D>();

        _initPos = transform.position;
    }
    public void SetterValues(GameObject obj, float distance, int damage, bool traverse, Vector2 target, int boss = 0, float speed = 0)
    {
        gameObj = obj;
        _distanceToAttack = distance;
        this.damage = damage;
        canTraverse = traverse;
        _target = target;

        if (speed != 0) speedMovement = speed;

        // BOSS = 0 = Lo envi� el Player
        // BOSS = 1 = Lo envi� un enemigo
        // BOSS = 2 = Lo envi� un Boss
        whoIsBoss = boss;

        if (boss != 0) { _collider.isTrigger = false; }
        else { _collider.isTrigger = true; }

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    /// <summary>
    /// Configura el pooling para este proyectil
    /// </summary>
    public void SetPool(ProjectilePool pool, GameObject prefab)
    {
        _pool = pool;
        _prefab = prefab;
        _usePooling = true;
    }

    /// <summary>
    /// Devuelve el proyectil al pool o lo destruye si no usa pooling
    /// </summary>
    private void ReturnToPool()
    {
        if (_usePooling && _pool != null && _prefab != null)
        {
            // Resetear valores antes de devolver al pool
            canAreaDamage = false;
            canTraverse = false;
            damage = 0;
            _rb2d.velocity = Vector2.zero;

            _pool.ReturnProjectile(_prefab, this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

        if(_distanceToAttack != 0)
        {
            if (Vector3.Distance(_initPos, transform.position) > _distanceToAttack)
            {
                ReturnToPool();
            }
        }

        _rb2d.velocity = _target * speedMovement;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && whoIsBoss == 0) { if (canAreaDamage) { Instantiate(spread, transform.position, Quaternion.identity); } }
        if (collision.CompareTag("Boss") && whoIsBoss == 0) { if (canAreaDamage) { Instantiate(spread, transform.position, Quaternion.identity); } }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && whoIsBoss != 0)
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(gameObj, damage);
            }
        }

        ReturnToPool();
    }
}
