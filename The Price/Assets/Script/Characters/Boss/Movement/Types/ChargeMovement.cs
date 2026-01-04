using UnityEngine;

/// <summary>
/// Patrón de movimiento de carga rápida hacia el jugador.
/// El boss acelera rápidamente en línea recta hacia la posición del jugador.
/// </summary>
public class ChargeMovement : TypeMovement
{
    [Header("Charge Settings")]
    [Tooltip("Multiplicador de velocidad durante la carga")]
    [Range(1.5f, 5f)]
    public float chargeSpeedMultiplier = 3f;

    [Tooltip("Distancia mínima al jugador (detiene la carga)")]
    public float stopDistance = 1f;

    [Tooltip("Efecto de rastro durante la carga (opcional)")]
    public GameObject chargeTrailEffect;

    private Vector3 chargeDirection;
    private GameObject trailInstance;
    private bool hasCalculatedDirection = false;

    public override void DataMove()
    {
        // Calcular dirección solo al inicio de la carga
        if (!hasCalculatedDirection)
        {
            Vector3 playerPos = _playerStats.transform.position;
            chargeDirection = (playerPos - transform.position).normalized;
            hasCalculatedDirection = true;

            // Activar efecto de rastro
            if (chargeTrailEffect != null)
            {
                trailInstance = Instantiate(chargeTrailEffect, transform);
            }
        }

        // Verificar distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, _playerStats.transform.position);
        if (distanceToPlayer < stopDistance)
        {
            StopCharge();
            return;
        }

        // Mover en línea recta con velocidad multiplicada
        transform.position += chargeDirection * speedMove * chargeSpeedMultiplier * Time.deltaTime;
    }

    private void StopCharge()
    {
        inMove = false;
        hasCalculatedDirection = false;

        // Limpiar efecto de rastro
        if (trailInstance != null)
        {
            Destroy(trailInstance);
            trailInstance = null;
        }
    }

    private void OnDisable()
    {
        // Limpiar trail si el objeto se desactiva
        if (trailInstance != null)
        {
            Destroy(trailInstance);
            trailInstance = null;
        }
    }
}
