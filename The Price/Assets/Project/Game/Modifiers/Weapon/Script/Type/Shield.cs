using UnityEngine;

public class Shield : WeaponSystem {

    [Tooltip("Intensidad del Empuje del Final HIT")] public float intensity;

    private void Awake() { anim = GetComponent<Animator>(); }
    public override void Attack()
    {
        anim.SetBool("Attack", true);
        _playerStats._canReceivedDamage = false;
    }
    public override void FinalHit()
    {
        anim.SetBool("Attack", true);
        gameObject.tag = "Push";
    }
}