using System.Collections;
using UnityEngine;

/// <summary>
/// Ataque que dispara proyectiles en espiral alrededor del boss.
/// Crea un patrón circular de proyectiles que rota.
/// </summary>
public class SpiralProjectileAttack : AttackBoss
{
    [Header("Spiral Data")]
    [Tooltip("Cantidad de proyectiles por ráfaga")]
    [Range(3, 20)]
    public int projectileCount = 8;

    [Tooltip("Distancia que recorren los proyectiles")]
    public float projectileDistance = 10f;

    [Tooltip("Velocidad de los proyectiles")]
    public float projectileSpeed = 5f;

    [Tooltip("Rotación inicial del patrón (grados)")]
    public float initialRotation = 0f;

    [Tooltip("Rotación adicional por cada ráfaga")]
    public float rotationIncrement = 45f;

    [Tooltip("Si es verdadero, los proyectiles pueden atravesar")]
    public bool canTraverse;

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            guideInScene = Instantiate(guideObj, enemyParent.transform.position, Quaternion.identity);

            AreaGuide areaGuide = guideInScene.GetComponent<AreaGuide>();
            if (areaGuide != null)
            {
                AreaGuideConfig config = new AreaGuideConfig
                {
                    origin = enemyParent.transform.position,
                    radius = projectileDistance
                };
                areaGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override IEnumerator LaunchedAttack()
    {
        Vector3 origin = GetPosition();
        float angleStep = 360f / projectileCount;

        // Aplicar rotación acumulativa para crear efecto espiral
        initialRotation += rotationIncrement;

        // Crear proyectiles en círculo
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (angleStep * i + initialRotation) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Projectile projectile = Instantiate(visualAttack.gameObject, origin, Quaternion.identity).GetComponent<Projectile>();
            projectile.SetterValues(enemyParent.gameObject, projectileDistance, GetDamage(), canTraverse, direction, 2, projectileSpeed);
        }

        yield return new WaitForSeconds(0.1f);
    }
}
