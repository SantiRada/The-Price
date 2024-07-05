using UnityEngine;

public class FallingAttack : AttackBoss {

    protected override Vector3 GetPosition() { return _player.transform.position; }
    protected override void LaunchedAttack()
    {
        ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();
        obj.SetValues(bossParent, GetDamage(), timeToDestroy);
    }
}