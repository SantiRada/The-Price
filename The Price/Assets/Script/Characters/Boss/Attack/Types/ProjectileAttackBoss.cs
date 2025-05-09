using System.Collections;
using UnityEngine;

public class ProjectileAttackBoss : AttackBoss {

    [Header("Data Projectile")]
    [Tooltip("Distancia m�xima que recorrer� el proyectil")]
    public float distanceProjectile;

    [Tooltip("Velocidad a la que se mover� el proyectil")]
    public float speedProjectile;

    [Tooltip("Si es verdadero, el proyectil puede atravesar objetos o enemigos")]
    public bool canTraverse;

    private void Start() { guideCreated += ChangeValuesGuide; }
    private void ChangeValuesGuide() { guideInScene.GetComponent<GuideProjectile>().SetSize(GetPlayerPosition(), distanceProjectile, true); }
    protected override Vector3 GetPosition() { return enemyParent.transform.position; }
    protected override IEnumerator LaunchedAttack()
    {
        // Instancia el proyectil y configura sus par�metros iniciales como da�o, direcci�n, velocidad, etc.
        Projectile obj = Instantiate(visualAttack.gameObject, GetPosition(), Quaternion.identity).GetComponent<Projectile>();
        obj.SetterValues(enemyParent.gameObject, distanceProjectile, GetDamage(), canTraverse, -(GetPosition() - GetPlayerPosition()), 2, speedProjectile);
        yield return new WaitForSeconds(0.1f);
    }
}
