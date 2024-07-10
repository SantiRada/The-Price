using UnityEngine;

public class DetectorPlayer : MonoBehaviour {

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            collision.GetComponent<PlayerStats>().TakeDamage(gameObject, damage);

            CancelAttack();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            collision.GetComponent<PlayerStats>().TakeDamage(gameObject, damage);

            CancelAttack();
        }
    }
    public void CancelAttack() { isAttacking = false; }
}
