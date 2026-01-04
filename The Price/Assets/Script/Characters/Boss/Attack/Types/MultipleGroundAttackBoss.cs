using System.Collections;
using UnityEngine;

public class MultipleGroundAttackBoss : AttackBoss {

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            Vector3 startPos = transform.position;
            guideInScene = Instantiate(guideObj, startPos, Quaternion.identity);

            // Configurar guía de múltiples puntos
            MultiPointGuide multiGuide = guideInScene.GetComponent<MultiPointGuide>();
            if (multiGuide != null)
            {
                MultiPointGuideConfig config = new MultiPointGuideConfig
                {
                    origin = startPos,
                    target = GetPlayerPosition(),
                    pointCount = countCreated,
                    size = 0.8f, // Tamaño de cada marcador
                    staggerAppearance = true, // Los puntos aparecen secuencialmente
                    staggerDelay = 0.15f
                };
                multiGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override Vector3 GetPosition() { return transform.position; }
    protected override IEnumerator LaunchedAttack()
    {
        // Direcci�n y origen fijados al inicio del ataque
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