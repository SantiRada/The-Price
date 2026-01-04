using System.Collections;
using UnityEngine;

public class FallingAttack : AttackBoss
{

    [Tooltip("Si es verdadero, el ataque caer� sobre la posici�n del jugador en lugar de sobre el enemigo")]
    public bool posInPlayer;

    protected override Vector3 GetPosition()
    {
        if (posInPlayer) return _player.transform.position;
        else return enemyParent.transform.position;
    }

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            Vector3 targetPos = GetPosition();
            guideInScene = Instantiate(guideObj, targetPos, Quaternion.identity);

            // Configurar guía de punto en la posición donde caerá el ataque
            PointGuide pointGuide = guideInScene.GetComponent<PointGuide>();
            if (pointGuide != null)
            {
                PointGuideConfig config = new PointGuideConfig
                {
                    target = targetPos,
                    size = 1.5f // Tamaño visual del marcador
                };
                pointGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override IEnumerator LaunchedAttack()
    {
        // Instancia m�ltiples objetos de ataque con un intervalo entre cada uno
        for (int i = 0; i < countCreated; i++)
        {
            ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();
            obj.SetValues(GetDamage(), timeToDestroy);
            yield return new WaitForSeconds(timeBetweenCreated);
        }
    }
}
