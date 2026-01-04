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

    private float currentAngle;
    private bool initialized = false;

    public override void DataMove()
    {
        if (direction == 0) direction = 1;

        Vector3 playerPos = _playerStats.transform.position;

        // Inicializar ángulo solo una vez
        if (!initialized)
        {
            Vector3 dirToPlayer = transform.position - playerPos;
            currentAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            initialized = true;
        }

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
        transform.position = Vector3.Lerp(transform.position, targetPos, speedMove * Time.deltaTime);
    }
}
