using System.Collections;
using UnityEngine;

public class FallingAttack : AttackBoss {

    public bool posInPlayer;

    protected override Vector3 GetPosition()
    {
        if(posInPlayer) return _player.transform.position;
        else return enemyParent.transform.position;
    }
    protected override IEnumerator LaunchedAttack()
    {
        for (int i = 0; i < countCreated; i++)
        {
            ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();
            obj.SetValues(GetDamage(), timeToDestroy);
            yield return new WaitForSeconds(timeBetweenCreated);
        }
    }
}