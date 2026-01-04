using System.Collections;
using UnityEngine;

public class ObjectSpread : MonoBehaviour {

    [Header("Push")]
    public bool canPush;
    public float pushForce;

    [Header("Content Damage")]
    public int damage;
    [HideInInspector] public int damageAffected = 0;

    [Header("Data State")]
    [HideInInspector] public TypeState state;
    [HideInInspector] public int countOfLoads;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // EMPUJAR AL ENEMIGO SI EST� MARCADO COMO "CANPUSH"
            if (canPush) StartCoroutine(PushEnemy(collision.gameObject));

            // AGREGAR ESTADO AL ENEMIGO COLISIONADO SI ESTA OPCI�N EST� ACTIVA
            if (state != TypeState.Null) collision.GetComponent<EnemyManager>().AddState(state, countOfLoads);

            // APLICAR DA�O AL ENEMIGO COLISIONADO
            collision.GetComponent<EnemyManager>().TakeDamage(damage);

            damageAffected += damage;
        }
    }
    private IEnumerator PushEnemy(GameObject obj)
    {
        Rigidbody2D enemyRigidbody = obj.GetComponent<Rigidbody2D>();

        Vector2 direction = (obj.transform.position - transform.position).normalized;

        float intensity = 1.15f;

        for (int i = 0; i < 20; i++)
        {
            if (enemyRigidbody != null)
            {
                obj.GetComponent<EnemyManager>().AddState(TypeState.Stun, 2);
                enemyRigidbody.AddForce(direction * (intensity / 20), ForceMode2D.Impulse);

                float maxVelocity = 10.0f;  // Adjust this value as needed
                enemyRigidbody.velocity = Vector2.ClampMagnitude(enemyRigidbody.velocity, maxVelocity);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
