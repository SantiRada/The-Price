using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Projectile : MonoBehaviour {

    [Header("Data Projectile")]
    public float speedMovement;
    private float _distanceToAttack;
    private int _dmg;
    private bool _canTraverse = false;

    private Vector2 _initPos;
    private Rigidbody2D _rb2d;
    private Vector2 _target;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _initPos = transform.position;
    }
    public void SetterValues(float distance, int damage, bool traverse, Vector2 target)
    {
        _distanceToAttack = distance;
        _dmg = damage;
        _canTraverse = traverse;
        _target = target;
    }
    private void Update()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

        if(_distanceToAttack != 0) if (Vector3.Distance(_initPos, transform.position) > _distanceToAttack) Destroy(gameObject);

        _rb2d.velocity = _target * speedMovement;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyManager>().TakeDamage(_dmg);

            if (!_canTraverse) Destroy(gameObject);
        }
    }
}
