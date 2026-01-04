using System.Collections;
using UnityEngine;

/// <summary>
/// Patrón de movimiento por teletransporte.
/// El boss desaparece y reaparece en una posición aleatoria cercana al jugador.
/// </summary>
public class TeleportMovement : TypeMovement
{
    [Header("Teleport Settings")]
    [Tooltip("Distancia mínima de teletransporte desde el jugador")]
    public float minTeleportDistance = 3f;

    [Tooltip("Distancia máxima de teletransporte desde el jugador")]
    public float maxTeleportDistance = 8f;

    [Tooltip("Efecto visual al desaparecer (opcional)")]
    public GameObject disappearEffect;

    [Tooltip("Efecto visual al aparecer (opcional)")]
    public GameObject appearEffect;

    [Tooltip("Tiempo que el boss permanece invisible")]
    public float invisibleDuration = 0.5f;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Move()
    {
        StartCoroutine(TeleportSequence());
    }

    public override void CancelMove()
    {
        StopAllCoroutines();
        if (_spriteRenderer != null) _spriteRenderer.enabled = true;
        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }

    private IEnumerator TeleportSequence()
    {
        // Efecto de desaparición
        if (disappearEffect != null)
        {
            Instantiate(disappearEffect, transform.position, Quaternion.identity);
        }

        // Hacer invisible al boss
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(invisibleDuration);

        // Calcular nueva posición aleatoria alrededor del jugador
        Vector3 playerPos = _player.transform.position;
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(minTeleportDistance, maxTeleportDistance);

        Vector3 newPosition = playerPos + new Vector3(
            Mathf.Cos(randomAngle) * randomDistance,
            Mathf.Sin(randomAngle) * randomDistance,
            0
        );

        // Teletransportar
        transform.position = newPosition;

        // Efecto de aparición
        if (appearEffect != null)
        {
            Instantiate(appearEffect, transform.position, Quaternion.identity);
        }

        // Hacer visible al boss
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = true;
        }

        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }
}
