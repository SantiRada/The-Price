using System.Collections;
using UnityEngine;

public class MultipleGroundAttackBoss : AttackBoss {

    private void Start() { guideCreated += ChangeValuesGuide; }
    private void ChangeValuesGuide()
    {
        guideInScene.GetComponent<GuideProjectile>().SetSize(GetPlayerPosition(), distanceToAttack, false);
    }
    protected override Vector3 GetPosition() { return transform.position; }
    protected override IEnumerator LaunchedAttack()
    {
        // Dirección y origen fijados al inicio del ataque
        Vector3 startPos = GetPosition();
        Vector3 direction = (GetPlayerPosition() - startPos).normalized;

        for (int i = 0; i < countCreated; i++)
        {
            Vector3 spawnPos = startPos + (direction * (1.2f * i));
            ObjectPerDamage obj = Instantiate(visualAttack.gameObject, spawnPos, Quaternion.identity).GetComponent<ObjectPerDamage>();
            obj.SetValues(GetDamage(), timeToDestroy);
            yield return new WaitForSeconds(timeBetweenCreated);
        }

        enemyParent.CanMove = true;
    }
}