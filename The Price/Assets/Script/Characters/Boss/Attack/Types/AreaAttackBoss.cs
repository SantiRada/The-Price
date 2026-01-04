using System.Collections;
using UnityEngine;

public class AreaAttackBoss : AttackBoss
{

    // Retorna la posici�n del enemigo como punto de origen del ataque
    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            Vector3 attackPos = enemyParent.transform.position;
            guideInScene = Instantiate(guideObj, attackPos, Quaternion.identity);

            // Configurar guía de área circular
            AreaGuide areaGuide = guideInScene.GetComponent<AreaGuide>();
            if (areaGuide != null)
            {
                AreaGuideConfig config = new AreaGuideConfig
                {
                    origin = attackPos,
                    radius = distanceToAttack // Radio del área de efecto
                };
                areaGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override IEnumerator LaunchedAttack()
    {
        // Instancia el objeto visual del ataque en la posici�n indicada
        ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();

        // Configura el da�o y tiempo de vida del objeto de ataque
        obj.SetValues(GetDamage(), timeToDestroy);

        // Peque�a espera tras lanzar el ataque
        yield return new WaitForSeconds(0.1f);
    }
}
