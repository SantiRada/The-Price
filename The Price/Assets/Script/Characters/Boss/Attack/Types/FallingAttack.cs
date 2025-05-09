using System.Collections;
using UnityEngine;

public class FallingAttack : AttackBoss
{

    [Tooltip("Si es verdadero, el ataque caerá sobre la posición del jugador en lugar de sobre el enemigo")]
    public bool posInPlayer;

    protected override Vector3 GetPosition()
    {
        if (posInPlayer) return _player.transform.position;
        else return enemyParent.transform.position;
    }

    protected override IEnumerator LaunchedAttack()
    {
        // Instancia múltiples objetos de ataque con un intervalo entre cada uno
        for (int i = 0; i < countCreated; i++)
        {
            ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();
            obj.SetValues(GetDamage(), timeToDestroy);
            yield return new WaitForSeconds(timeBetweenCreated);
        }
    }
}
