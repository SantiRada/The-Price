using UnityEngine;

/// <summary>
/// Patrón de movimiento de retirada.
/// El boss se aleja del jugador cuando está demasiado cerca.
/// Útil para bosses de rango que necesitan mantener distancia.
/// </summary>
public class RetreatMovement : TypeMovement
{
    [Header("Retreat Settings")]
    [Tooltip("Distancia de seguridad que el boss intenta mantener")]
    public float safeDistance = 6f;

    [Tooltip("Si es verdadero, retrocede en línea recta. Si es falso, con esquiva lateral.")]
    public bool straightRetreat = false;

    [Tooltip("Ángulo de desvío lateral si no es retirada recta (grados)")]
    [Range(0, 90)]
    public float dodgeAngle = 45f;

    private Vector3 retreatDirection;
    private bool hasCalculatedDirection = false;

    public override void DataMove()
    {
        // Calcular dirección de retirada solo al inicio
        if (!hasCalculatedDirection)
        {
            Vector3 playerPos = _playerStats.transform.position;
            retreatDirection = (transform.position - playerPos).normalized;

            // Si no es retirada recta, añadir componente lateral
            if (!straightRetreat)
            {
                int lateralDir = Random.Range(0, 2) == 0 ? 1 : -1;
                float angleRad = dodgeAngle * Mathf.Deg2Rad * lateralDir;

                float cos = Mathf.Cos(angleRad);
                float sin = Mathf.Sin(angleRad);
                Vector3 rotatedDir = new Vector3(
                    retreatDirection.x * cos - retreatDirection.y * sin,
                    retreatDirection.x * sin + retreatDirection.y * cos,
                    0
                );

                retreatDirection = rotatedDir.normalized;
            }

            hasCalculatedDirection = true;
        }

        // Verificar si ya está a distancia segura
        float currentDistance = Vector3.Distance(transform.position, _playerStats.transform.position);
        if (currentDistance >= safeDistance)
        {
            inMove = false;
            hasCalculatedDirection = false;
            return;
        }

        // Mover en dirección de retirada
        transform.position += retreatDirection * speedMove * Time.deltaTime;
    }
}
