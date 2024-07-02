using UnityEngine;

public class Area : WeaponSystem {

    public GameObject spread;
    public GameObject spreadFinalHit;

    private SpreadDamage _spreadDmg;

    private void Awake() { _spreadDmg = GetComponent<SpreadDamage>(); }
    public override void Attack()
    {
        if(_spreadDmg.spread != null)
        {
            _spreadDmg.StopCoroutine("MoveSpread");
            Destroy(_spreadDmg.spread);
        }

        _spreadDmg.objSpread = spread;
        _spreadDmg.SpreadTypeDamage();
    }
    public override void FinalHit()
    {
        if (_spreadDmg.spread != null)
        {
            _spreadDmg.StopCoroutine("MoveSpread");
            Destroy(_spreadDmg.spread);
        }

        _spreadDmg.objSpread = spreadFinalHit;
        _spreadDmg.SpreadTypeDamage();
    }
}
