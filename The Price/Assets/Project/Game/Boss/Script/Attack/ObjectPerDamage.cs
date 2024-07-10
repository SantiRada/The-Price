using UnityEngine;

public class ObjectPerDamage : MonoBehaviour {

    [Header("Stats")]
    public bool canRemain = false;
    private int damage;
    private float timer;

    [Header("Private Content")]
    private Collider2D _col2D;

    private void Start() { _col2D = GetComponent<Collider2D>(); }
    public void SetValues(int dmg, float time)
    {
        damage = dmg;
        timer = time;

        Destroy(gameObject, timer);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage(gameObject, damage);

            if (!canRemain) { Destroy(gameObject); }
            else { _col2D.enabled = false; }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) { if (collision.gameObject.CompareTag("Player")) { _col2D.enabled = true; } }
}