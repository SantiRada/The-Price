using System.Collections;
using UnityEngine;

/// <summary>
/// Ataque de proyectil que persigue al jugador.
/// El proyectil ajusta su dirección continuamente para seguir al objetivo.
/// </summary>
public class HomingProjectileAttack : AttackBoss
{
    [Header("Homing Data")]
    [Tooltip("Distancia máxima que recorrerá el proyectil")]
    public float distanceProjectile;

    [Tooltip("Velocidad base del proyectil")]
    public float speedProjectile;

    [Tooltip("Velocidad de rotación para seguir al jugador (grados por segundo)")]
    public float homingSpeed = 180f;

    [Tooltip("Si es verdadero, el proyectil puede atravesar objetos")]
    public bool canTraverse;

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            guideInScene = Instantiate(guideObj, enemyParent.transform.position, Quaternion.identity);

            LinearGuide linearGuide = guideInScene.GetComponent<LinearGuide>();
            if (linearGuide != null)
            {
                LinearGuideConfig config = new LinearGuideConfig
                {
                    origin = enemyParent.transform.position,
                    target = GetPlayerPosition(),
                    size = distanceProjectile,
                    targetTransform = _player.transform // Seguir al jugador visualmente
                };
                linearGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override IEnumerator LaunchedAttack()
    {
        Projectile obj = Instantiate(visualAttack.gameObject, GetPosition(), Quaternion.identity).GetComponent<Projectile>();
        obj.SetterValues(enemyParent.gameObject, distanceProjectile, GetDamage(), canTraverse, -(GetPosition() - GetPlayerPosition()), 2, speedProjectile);

        // Aquí se podría añadir un componente HomingBehavior al proyectil para que siga al jugador
        // HomingBehavior homing = obj.gameObject.AddComponent<HomingBehavior>();
        // homing.target = _player.transform;
        // homing.homingSpeed = homingSpeed;

        yield return new WaitForSeconds(0.1f);
    }
}
