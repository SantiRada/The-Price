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

    private SpriteRenderer _spriteRenderer;
    private bool hasTeleported = false;
    private float teleportDelay = 0.3f;
    private float delayTimer = 0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void DataMove()
    {
        if (!hasTeleported)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer < teleportDelay)
            {
                // Fase de desaparición
                if (delayTimer < 0.1f && disappearEffect != null)
                {
                    Instantiate(disappearEffect, transform.position, Quaternion.identity);
                }

                // Hacer invisible gradualmente
                if (_spriteRenderer != null && delayTimer > 0.05f)
                {
                    _spriteRenderer.enabled = false;
                }
            }
            else
            {
                // Teletransportar
                Vector3 playerPos = _playerStats.transform.position;
                float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float randomDistance = Random.Range(minTeleportDistance, maxTeleportDistance);

                transform.position = playerPos + new Vector3(
                    Mathf.Cos(randomAngle) * randomDistance,
                    Mathf.Sin(randomAngle) * randomDistance,
                    0
                );

                // Efecto de aparición
                if (appearEffect != null)
                {
                    Instantiate(appearEffect, transform.position, Quaternion.identity);
                }

                // Hacer visible
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.enabled = true;
                }

                hasTeleported = true;
                inMove = false; // Terminar movimiento
                delayTimer = 0f;
            }
        }
    }
}
