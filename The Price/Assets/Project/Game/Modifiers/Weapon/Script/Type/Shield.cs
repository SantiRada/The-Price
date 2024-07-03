using UnityEngine;

public class Shield : WeaponSystem {

    [Tooltip("Intensidad del Empuje del Final HIT")] public float intensity;

    public override void Attack() { _player.CanReceivedDamage = false; }
    public override void FinalHit() { gameObject.tag = "Push"; }
}