using UnityEngine;

public class Area : WeaponSystem {

    public GameObject spread;
    public GameObject spreadFinalHit;

    private SpreadDamage _spreadDmg;

    public override void Attack()
    {
        _spreadDmg.objSpread = spread;
        _spreadDmg.SpreadTypeDamage();
    }
    public override void FinalHit()
    {
        _spreadDmg.objSpread = spreadFinalHit;
        _spreadDmg.SpreadTypeDamage();
    }
}
