using System.Collections;
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

    [Tooltip("Duración del movimiento de retirada")]
    public float retreatDuration = 1.5f;

    [Tooltip("Si es verdadero, el boss retrocede en línea recta. Si es falso, retrocede con esquiva lateral.")]
    public bool straightRetreat = false;

    [Tooltip("Ángulo de desvío lateral si no es retirada recta (grados)")]
    [Range(0, 90)]
    public float dodgeAngle = 45f;

    public override void Move()
    {
        StartCoroutine(RetreatSequence());
    }

    public override void CancelMove()
    {
        StopAllCoroutines();
        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }

    private IEnumerator RetreatSequence()
    {
        float elapsed = 0f;

        // Calcular dirección de retirada
        Vector3 playerPos = _player.transform.position;
        Vector3 retreatDirection = (transform.position - playerPos).normalized;

        // Si no es retirada recta, añadir componente lateral
        if (!straightRetreat)
        {
            // Elegir dirección lateral aleatoria
            int lateralDir = Random.Range(0, 2) == 0 ? 1 : -1;
            float angleRad = dodgeAngle * Mathf.Deg2Rad * lateralDir;

            // Rotar la dirección de retirada
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);
            Vector3 rotatedDir = new Vector3(
                retreatDirection.x * cos - retreatDirection.y * sin,
                retreatDirection.x * sin + retreatDirection.y * cos,
                0
            );

            retreatDirection = rotatedDir.normalized;
        }

        while (elapsed < retreatDuration)
        {
            if (Pause.state != State.Game || LoadingScreen.inLoading)
            {
                yield return null;
                continue;
            }

            // Verificar si ya está a distancia segura
            float currentDistance = Vector3.Distance(transform.position, _player.transform.position);
            if (currentDistance >= safeDistance)
            {
                break;
            }

            // Mover en dirección de retirada
            Vector3 movement = retreatDirection * _bossManager.speed * Time.deltaTime;
            transform.position += movement;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }
}
