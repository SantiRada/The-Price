using System.Collections;
using UnityEngine;

/// <summary>
/// Patrón de movimiento donde el boss circula alrededor del jugador.
/// Mantiene una distancia constante mientras orbita.
/// </summary>
public class CircleAroundPlayer : TypeMovement
{
    [Header("Circle Settings")]
    [Tooltip("Radio de la órbita alrededor del jugador")]
    public float orbitRadius = 5f;

    [Tooltip("Velocidad angular (grados por segundo)")]
    public float angularSpeed = 60f;

    [Tooltip("Dirección de rotación (1 = horario, -1 = antihorario)")]
    [Range(-1, 1)]
    public int direction = 1;

    [Tooltip("Duración del movimiento circular")]
    public float moveDuration = 3f;

    private float currentAngle;

    public override void Move()
    {
        if (direction == 0) direction = 1; // Asegurar que no sea 0
        StartCoroutine(CircleMovement());
    }

    public override void CancelMove()
    {
        StopAllCoroutines();
        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }

    private IEnumerator CircleMovement()
    {
        Vector3 playerPos = _player.transform.position;

        // Calcular ángulo inicial basado en posición actual
        Vector3 dirToPlayer = transform.position - playerPos;
        currentAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            if (Pause.state != State.Game || LoadingScreen.inLoading)
            {
                yield return null;
                continue;
            }

            // Actualizar posición del jugador para seguir su movimiento
            playerPos = _player.transform.position;

            // Incrementar ángulo
            currentAngle += angularSpeed * direction * Time.deltaTime;

            // Calcular nueva posición en el círculo
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 targetPos = playerPos + new Vector3(
                Mathf.Cos(radians) * orbitRadius,
                Mathf.Sin(radians) * orbitRadius,
                0
            );

            // Mover hacia la posición objetivo
            transform.position = Vector2.MoveTowards(transform.position, targetPos, _bossManager.speed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }
}
