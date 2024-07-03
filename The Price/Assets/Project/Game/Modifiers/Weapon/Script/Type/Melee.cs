using UnityEngine;

public class Melee : WeaponSystem {

    public override void Attack() { gameObject.tag = "Proyectile"; }
    public override void FinalHit() { gameObject.tag = "Proyectile"; }
}
