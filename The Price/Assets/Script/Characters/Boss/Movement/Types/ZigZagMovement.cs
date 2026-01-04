using UnityEngine;

/// <summary>
/// Patrón de movimiento en zig-zag hacia el jugador.
/// Crea un movimiento impredecible mientras se acerca.
/// </summary>
public class ZigZagMovement : TypeMovement
{
    [Header("Zig-Zag Settings")]
    [Tooltip("Amplitud del zig-zag (distancia lateral)")]
    public float zigZagAmplitude = 2f;

    [Tooltip("Frecuencia del zig-zag (cambios por segundo)")]
    public float zigZagFrequency = 2f;

    [Tooltip("Si es verdadero, se mueve hacia el jugador. Si es falso, se aleja.")]
    public bool moveTowardsPlayer = true;

    [Tooltip("Distancia mínima al jugador (detiene el movimiento)")]
    public float minDistance = 2f;

    private float zigZagTimer;
    private int zigZagDirection = 1;
    private bool initialized = false;

    public override void DataMove()
    {
        if (!initialized)
        {
            zigZagDirection = Random.Range(0, 2) == 0 ? 1 : -1;
            initialized = true;
        }

        // Verificar si llegó a la distancia mínima
        float distanceToPlayer = Vector3.Distance(transform.position, _playerStats.transform.position);
        if (moveTowardsPlayer && distanceToPlayer < minDistance)
        {
            inMove = false;
            initialized = false;
            return;
        }

        // Cambiar dirección del zig-zag periódicamente
        zigZagTimer += Time.deltaTime;
        float changeInterval = 1f / zigZagFrequency;

        if (zigZagTimer >= changeInterval)
        {
            zigZagDirection *= -1;
            zigZagTimer = 0f;
        }

        // Calcular dirección hacia/desde el jugador
        Vector3 playerPos = _playerStats.transform.position;
        Vector3 directionToPlayer = (playerPos - transform.position).normalized;

        if (!moveTowardsPlayer)
        {
            directionToPlayer = -directionToPlayer;
        }

        // Calcular dirección perpendicular para el zig-zag
        Vector3 perpendicular = new Vector3(-directionToPlayer.y, directionToPlayer.x, 0);

        // Combinar movimiento hacia adelante con desplazamiento lateral
        Vector3 movement = (directionToPlayer + perpendicular * zigZagDirection * 0.5f).normalized;

        // Aplicar movimiento
        transform.position += movement * speedMove * Time.deltaTime;
    }
}
