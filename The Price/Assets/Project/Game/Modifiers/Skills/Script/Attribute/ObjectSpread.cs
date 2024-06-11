using System.Collections;
using UnityEngine;

public class ObjectSpread : MonoBehaviour {

    public bool canPush;
    public float pushForce;

    private PlayerStats _playerStats;

    private void Start()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (canPush) StartCoroutine(PushEnemy(collision.gameObject));

            collision.GetComponent<EnemyManager>().TakeDamage((int)_playerStats.GetterStats(6, false));
        }
    }
    private IEnumerator PushEnemy(GameObject obj)
    {
        Rigidbody2D enemyRigidbody = obj.GetComponent<Rigidbody2D>();

        Vector2 direction = (obj.transform.position - transform.position).normalized;

        for (int i = 0; i < 20; i++)
        {
            if (enemyRigidbody != null) if(enemyRigidbody != null) enemyRigidbody.AddForce(direction * pushForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
