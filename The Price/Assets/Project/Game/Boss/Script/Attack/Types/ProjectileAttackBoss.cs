using UnityEngine;

public class ProjectileAttackBoss : AttackBoss {

    [Header("Data Projectile")]
    public float distanceProjectile;
    public float speedProjectile;
    public bool canTraverse;

    private void Start() { guideCreated += ChangeValuesGuide; }
    private void ChangeValuesGuide() { guideInScene.GetComponent<GuideProjectile>().SetSize(GetPlayerPosition(), distanceProjectile); }
    protected override Vector3 GetPosition() { return bossParent.transform.position; }
    protected override void LaunchedAttack()
    {
        Projectile obj = Instantiate(visualAttack.gameObject, GetPosition(), Quaternion.identity).GetComponent<Projectile>();
        obj.SetterValues(bossParent.gameObject, distanceProjectile, GetDamage(), canTraverse, GetPlayerPosition(), 2, speedProjectile);
    }
}