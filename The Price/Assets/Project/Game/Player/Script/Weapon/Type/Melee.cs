using UnityEngine;

public class Melee : WeaponSystem {

    public override void Attack()
    {
        damage = damageWeapon;
        gameObject.tag = "Proyectile";
    }
    public override void FinalHit()
    {
        damage = damageFinalHit;
        gameObject.tag = "Proyectile";
    }
}
