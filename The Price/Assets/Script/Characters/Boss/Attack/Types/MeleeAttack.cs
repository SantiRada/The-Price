using System.Collections;
using UnityEngine;

/// <summary>
/// Ataque cuerpo a cuerpo del boss.
/// El boss debe estar cerca del jugador para ejecutar este ataque.
/// </summary>
public class MeleeAttack : AttackBoss
{
    [Header("Melee Data")]
    [Tooltip("Radio del ataque cuerpo a cuerpo")]
    public float meleeRadius = 2f;

    [Tooltip("Ángulo del ataque (180 = semicírculo frontal, 360 = todo alrededor)")]
    [Range(0, 360)]
    public float attackAngle = 180f;

    [Tooltip("Duración del hitbox activo")]
    public float hitboxDuration = 0.3f;

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            guideInScene = Instantiate(guideObj, enemyParent.transform.position, Quaternion.identity);

            // Usar guía de área para mostrar el radio del ataque
            AreaGuide areaGuide = guideInScene.GetComponent<AreaGuide>();
            if (areaGuide != null)
            {
                AreaGuideConfig config = new AreaGuideConfig
                {
                    origin = enemyParent.transform.position,
                    radius = meleeRadius
                };
                areaGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override IEnumerator LaunchedAttack()
    {
        // Instanciar el objeto de ataque en la posición del boss
        ObjectPerDamage obj = Instantiate(visualAttack.gameObject, GetPosition(), Quaternion.identity).GetComponent<ObjectPerDamage>();
        obj.SetValues(GetDamage(), hitboxDuration);

        // Escalar el objeto según el radio
        obj.transform.localScale = Vector3.one * (meleeRadius * 2);

        // Si el ataque es direccional, rotarlo hacia el jugador
        if (attackAngle < 360)
        {
            Vector3 direction = (GetPlayerPosition() - GetPosition()).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        yield return new WaitForSeconds(0.1f);
    }
}
