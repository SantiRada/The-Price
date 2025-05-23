using UnityEngine;

public enum TypeAsset { Trigger, Collision, Clicked, Gold }
public class DecorationAsset : MonoBehaviour {

    [SerializeField] private TypeAsset _typeAsset;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Activate()
    {
        anim.SetBool("Activate", true);
        Destroy(this);
    }
    // ---- TRIGGER ELEMENT ------ //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { if(_typeAsset == TypeAsset.Trigger) { Activate(); } }
        if (collision.CompareTag("Weapon")) { Activate(); }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon")) { Activate(); }
    }
    // ---- COLLISION ELEMENT ---- //
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_typeAsset == TypeAsset.Collision) Activate();

            if(_typeAsset == TypeAsset.Gold || _typeAsset == TypeAsset.Clicked)
            {
                if (collision.gameObject.GetComponent<PlayerMovement>().isDashing) Activate();
            }
        }
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Activate();
            if (_typeAsset == TypeAsset.Gold) ManagerGold.CreateGold(transform.position, CountGold.Small);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon")) { Activate(); }
    }
}
