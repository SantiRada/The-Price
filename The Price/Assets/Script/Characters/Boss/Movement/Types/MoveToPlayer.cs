using UnityEngine;

public class MoveToPlayer : TypeMovement {

    [Tooltip("Distancia máxima hasta dónde puede acercarse")] public float minDistanceToPlayer;

    public override void DataMove()
    {
        transform.position = Vector3.Lerp(transform.position, _playerStats.transform.position, 0.5f * speedMove * Time.deltaTime);

        if (Vector3.Distance(transform.position, _playerStats.transform.position) < minDistanceToPlayer) { inMove = false; }
    }
}
