using UnityEngine;

public class Melee : WeaponSystem {

    private void Awake() { anim = GetComponent<Animator>(); }
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
