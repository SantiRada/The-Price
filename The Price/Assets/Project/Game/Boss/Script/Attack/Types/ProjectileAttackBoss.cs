using System.Collections;
using UnityEngine;

public class ProjectileAttackBoss : AttackBoss {

    [Header("Data Projectile")]
    public float distanceProjectile;
    public float speedProjectile;
    public bool canTraverse;

    private void Start() { guideCreated += ChangeValuesGuide; }
    private void ChangeValuesGuide() { guideInScene.GetComponent<GuideProjectile>().SetSize(GetPlayerPosition(), distanceProjectile, true); }
    protected override Vector3 GetPosition() { return bossParent.transform.position; }
    protected override IEnumerator LaunchedAttack()
    {
        Projectile obj = Instantiate(visualAttack.gameObject, GetPosition(), Quaternion.identity).GetComponent<Projectile>();
        obj.SetterValues(bossParent.gameObject, distanceProjectile, GetDamage(), canTraverse, -(GetPosition() - GetPlayerPosition()), 2, speedProjectile);
        yield return new WaitForSeconds(0.1f);
    }
}