using UnityEngine;

public class DistanceEnemy : EnemyManager {

    [Header("Distance Data")]
    public GameObject projectile;
    [Tooltip("Velocidad del Proyectil")] public float speedAttack;
    [Tooltip("Distancia del proyectil antes de ser destruido")] public float distanceToAttack;

    public override void SpecificAttack(int index = -1)
    {
        Projectile pr = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();

        Vector2 direction = (Vector2)transform.position - (Vector2)_playerStats.transform.position;

        if(pr != null) pr.SetterValues(gameObject, distanceToAttack, damage, true, -direction, 1, speedAttack);
    }
}
