using UnityEngine;

public class Melee : WeaponSystem {

    private void Awake() { anim = GetComponent<Animator>(); }
    private void Start()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }
    public override void Attack()
    {
        anim.SetBool("Attack", true);
        gameObject.tag = "Proyectile";
    }
    public override void FinalHit()
    {
        anim.SetBool("Attack", true);
        gameObject.tag = "Proyectile";
    }
}
