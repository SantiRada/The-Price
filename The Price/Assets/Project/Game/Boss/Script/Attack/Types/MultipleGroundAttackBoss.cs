using System.Collections;
using UnityEngine;

public class MultipleGroundAttackBoss : AttackBoss {

    private int index = 0;

    private void Start() { guideCreated += ChangeValuesGuide; }
    private void ChangeValuesGuide()
    {
        index = 1;
        guideInScene.GetComponent<GuideProjectile>().SetSize(GetPlayerPosition(), distanceToAttack, false);
    }
    protected override Vector3 GetPosition() { return enemyParent.transform.position; }
    protected override IEnumerator LaunchedAttack()
    {
        index = 0;
        for (int i = 0; i < countCreated; i++)
        {
            ObjectPerDamage obj = Instantiate(visualAttack.gameObject, CalculatePos(), Quaternion.identity).GetComponent<ObjectPerDamage>();
            obj.SetValues(GetDamage(), timeToDestroy);
            yield return new WaitForSeconds(timeBetweenCreated);
        }
    }
    private Vector3 CalculatePos()
    {
        Vector3 difference = (GetPlayerPosition() - GetPosition()).normalized;

        Vector3 newPos = GetPosition() + (difference * (1.2f * index));

        index++;

        return newPos;
    }
}