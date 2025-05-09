using System.Collections;
using UnityEngine;

public class AreaAttackBoss : AttackBoss
{

    // Retorna la posición del enemigo como punto de origen del ataque
    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override IEnumerator LaunchedAttack()
    {
        // Instancia el objeto visual del ataque en la posición indicada
        ObjectPerDamage obj = Instantiate(visualAttack.gameObject, posInScene, Quaternion.identity).GetComponent<ObjectPerDamage>();

        // Configura el daño y tiempo de vida del objeto de ataque
        obj.SetValues(GetDamage(), timeToDestroy);

        // Pequeña espera tras lanzar el ataque
        yield return new WaitForSeconds(0.1f);
    }
}
