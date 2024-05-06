using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Data")]
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToDestroy = 1f;

    [Header("Private Data")]
    private float _distanceToDestroy;
    private Rigidbody2D _rigidbody2D;
    private Vector3 _initialPos;
    private uint _damage = 0;

    private void LateUpdate()
    {
        if (Vector3.Distance(transform.position, _initialPos) >= _distanceToDestroy) Destroy(gameObject);
    }
    public void LaunchProjectile(Vector2 direction, float rangeToShoot, uint dmg)
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _initialPos = transform.position;

        _damage = dmg;
        _distanceToDestroy = rangeToShoot;
        _rigidbody2D.velocity = direction * _speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player" && collision.tag != "Weapon" && collision.tag != "Crosshair")
        {
            Destroy(gameObject, _timeToDestroy);
        }
    }
}
