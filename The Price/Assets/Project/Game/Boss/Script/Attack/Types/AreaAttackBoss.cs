using System.Collections;
using UnityEngine;

public class AreaAttackBoss : AttackBoss {

    protected override Vector3 GetPosition() { return bossParent.transform.position; }
    protected override IEnumerator LaunchedAttack()
    {
        ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();
        obj.SetValues(bossParent, GetDamage(), timeToDestroy);
        yield return new WaitForSeconds(0.1f);
    }
}
