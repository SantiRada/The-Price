using System.Collections;
using UnityEngine;

/// <summary>
/// Patrón de movimiento en zig-zag hacia el jugador.
/// Crea un movimiento impredecible mientras se acerca.
/// </summary>
public class ZigZagMovement : TypeMovement
{
    [Header("Zig-Zag Settings")]
    [Tooltip("Distancia lateral de cada zig o zag")]
    public float zigZagAmplitude = 2f;

    [Tooltip("Frecuencia del zig-zag (cambios por segundo)")]
    public float zigZagFrequency = 2f;

    [Tooltip("Duración del movimiento")]
    public float moveDuration = 2f;

    [Tooltip("Si es verdadero, el boss se mueve hacia el jugador. Si es falso, se aleja.")]
    public bool moveTowardsPlayer = true;

    private float zigZagTimer;
    private int zigZagDirection = 1;

    public override void Move()
    {
        zigZagTimer = 0f;
        zigZagDirection = Random.Range(0, 2) == 0 ? 1 : -1; // Dirección inicial aleatoria
        StartCoroutine(ZigZagMovementCoroutine());
    }

    public override void CancelMove()
    {
        StopAllCoroutines();
        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }

    private IEnumerator ZigZagMovementCoroutine()
    {
        float elapsed = 0f;
        float changeInterval = 1f / zigZagFrequency;

        while (elapsed < moveDuration)
        {
            if (Pause.state != State.Game || LoadingScreen.inLoading)
            {
                yield return null;
                continue;
            }

            // Cambiar dirección del zig-zag periódicamente
            zigZagTimer += Time.deltaTime;
            if (zigZagTimer >= changeInterval)
            {
                zigZagDirection *= -1;
                zigZagTimer = 0f;
            }

            // Calcular dirección hacia/desde el jugador
            Vector3 playerPos = _player.transform.position;
            Vector3 directionToPlayer = (playerPos - transform.position).normalized;

            if (!moveTowardsPlayer)
            {
                directionToPlayer = -directionToPlayer;
            }

            // Calcular dirección perpendicular para el zig-zag
            Vector3 perpendicular = new Vector3(-directionToPlayer.y, directionToPlayer.x, 0);

            // Combinar movimiento hacia adelante con desplazamiento lateral
            Vector3 movement = (directionToPlayer + perpendicular * zigZagDirection * zigZagAmplitude * 0.5f).normalized;

            // Aplicar movimiento
            Vector3 newPosition = transform.position + movement * _bossManager.speed * Time.deltaTime;
            transform.position = newPosition;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }
}
