using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Data Projectile")]
    public float speedMovement;
    private float _distanceToAttack;
    private int _dmg;
    private bool _canTraverse = false;
    private int whoIsBoss = 0;
    private GameObject gameObj;

    private Vector2 _initPos;
    private Rigidbody2D _rb2d;
    private BoxCollider2D _collider;
    private Vector2 _target;

    private void OnEnable()
    {
        _collider = GetComponent<BoxCollider2D>();
        _rb2d = GetComponent<Rigidbody2D>();
        _initPos = transform.position;
    }
    public void SetterValues(GameObject obj, float distance, int damage, bool traverse, Vector2 target, int boss = 0, float speed = 0)
    {
        gameObj = obj;
        _distanceToAttack = distance;
        _dmg = damage;
        _canTraverse = traverse;
        _target = target;

        if (speed != 0) speedMovement = speed;

        // BOSS = 0 = Lo envió el Player // BOSS = 1 = Lo envió un enemigo //
        whoIsBoss = boss;

        if (boss == 1) _collider.isTrigger = false;
        else _collider.isTrigger = true;
    }
    private void Update()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

        if(_distanceToAttack != 0) if (Vector3.Distance(_initPos, transform.position) > _distanceToAttack) Destroy(gameObject);

        _rb2d.velocity = _target * speedMovement;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && whoIsBoss == 0)
        {
            collision.GetComponent<EnemyManager>().TakeDamage(_dmg);

            if (!_canTraverse) Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && whoIsBoss == 1) collision.gameObject.GetComponent<PlayerStats>().TakeDamage(gameObj, _dmg);

        Destroy(gameObject);
    }
}
