using System.Collections;
using UnityEngine;

public class ObjectSpread : MonoBehaviour {

    public bool canPush;
    public float pushForce;

    private PlayerStats _playerStats;
    [HideInInspector] public int damageAffected = 0;

    [Header("Data State")]
    [HideInInspector] public TypeState state;
    [HideInInspector] public int countOfLoads;

    private void Start()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // EMPUJAR AL ENEMIGO SI ESTÁ MARCADO COMO "CANPUSH"
            if (canPush) StartCoroutine(PushEnemy(collision.gameObject));

            // AGREGAR ESTADO AL ENEMIGO COLISIONADO SI ESTA OPCIÓN ESTÁ ACTIVA
            if (state != TypeState.Null) collision.GetComponent<EnemyManager>().AddState(state, countOfLoads);

            // APLICAR DAÑO AL ENEMIGO COLISIONADO
            collision.GetComponent<EnemyManager>().TakeDamage((int)_playerStats.GetterStats(4, false));

            damageAffected += (int)(_playerStats.GetterStats(4, false));
        }
    }
    private IEnumerator PushEnemy(GameObject obj)
    {
        Rigidbody2D enemyRigidbody = obj.GetComponent<Rigidbody2D>();

        Vector2 direction = (obj.transform.position - transform.position).normalized;

        for (int i = 0; i < 20; i++)
        {
            if (enemyRigidbody != null)
            {
                obj.GetComponent<EnemyManager>().AddState(TypeState.Stun, 2);
                if (enemyRigidbody != null) enemyRigidbody.AddForce(direction * pushForce, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
