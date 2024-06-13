using UnityEngine;

public class DistanceEnemy : EnemyManager {

    [Header("Distance Data")]
    public GameObject projectile;

    public override void Attack()
    {
        CanAttack = false;

        Projectile pr = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();

        Vector2 direction = (Vector2)transform.position - (Vector2)_player.transform.position;

        if(pr != null) pr.SetterValues(gameObject, distanceToAttack, damage, true, -direction, 1, speedAttack);
    }
}
