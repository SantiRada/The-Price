using System.Collections;
using UnityEngine;

public class AreaAttackBoss : AttackBoss
{

    // Retorna la posici�n del enemigo como punto de origen del ataque
    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

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
