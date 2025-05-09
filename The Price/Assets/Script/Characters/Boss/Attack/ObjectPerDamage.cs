using UnityEngine;

public class ObjectPerDamage : MonoBehaviour
{

    [Header("Stats")]
    [Tooltip("Si es verdadero, el objeto no se destruye al impactar, pero deja de hacer daño tras el primer contacto")]
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

        // Destruye el objeto tras el tiempo indicado
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage(gameObject, damage);

            // Si el objeto no puede permanecer, se destruye tras impactar
            if (!canRemain) { Destroy(gameObject); }
            else { _col2D.enabled = false; } // Desactiva el collider para evitar múltiples daños
        }
    }

    private void OnTriggerExit2D(Collider2D collision) { if (collision.gameObject.CompareTag("Player")) { _col2D.enabled = true; } }
}
