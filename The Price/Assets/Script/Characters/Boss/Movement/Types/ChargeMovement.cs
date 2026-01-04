using System.Collections;
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

    [Tooltip("Duración de la carga")]
    public float chargeDuration = 1f;

    [Tooltip("Tiempo de preparación antes de la carga")]
    public float windupTime = 0.5f;

    [Tooltip("Distancia máxima de la carga")]
    public float maxChargeDistance = 10f;

    [Tooltip("Si es verdadero, el boss se detiene al alcanzar al jugador")]
    public bool stopAtPlayer = true;

    [Header("Visual Feedback")]
    [Tooltip("Efecto visual durante la preparación (opcional)")]
    public GameObject windupEffect;

    [Tooltip("Efecto de rastro durante la carga (opcional)")]
    public GameObject chargeTrailEffect;

    public override void Move()
    {
        StartCoroutine(ChargeSequence());
    }

    public override void CancelMove()
    {
        StopAllCoroutines();
        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }

    private IEnumerator ChargeSequence()
    {
        // Fase de preparación
        if (windupTime > 0)
        {
            if (windupEffect != null)
            {
                GameObject windup = Instantiate(windupEffect, transform.position, Quaternion.identity, transform);
                Destroy(windup, windupTime);
            }

            // Aquí se podría añadir animación de preparación
            // anim.SetBool("WindingUp", true);

            yield return new WaitForSeconds(windupTime);

            // anim.SetBool("WindingUp", false);
        }

        // Calcular dirección y distancia
        Vector3 startPos = transform.position;
        Vector3 playerPos = _player.transform.position;
        Vector3 chargeDirection = (playerPos - startPos).normalized;
        float distanceToPlayer = Vector3.Distance(startPos, playerPos);
        float chargeDistance = Mathf.Min(distanceToPlayer, maxChargeDistance);

        // Activar efecto de rastro si existe
        GameObject trail = null;
        if (chargeTrailEffect != null)
        {
            trail = Instantiate(chargeTrailEffect, transform);
        }

        // Ejecutar la carga
        float elapsed = 0f;
        float chargeSpeed = _bossManager.speed * chargeSpeedMultiplier;
        Vector3 targetPos = startPos + chargeDirection * chargeDistance;

        while (elapsed < chargeDuration)
        {
            if (Pause.state != State.Game || LoadingScreen.inLoading)
            {
                yield return null;
                continue;
            }

            // Mover en línea recta
            transform.position = Vector2.MoveTowards(transform.position, targetPos, chargeSpeed * Time.deltaTime);

            // Detener si llegó al objetivo
            if (stopAtPlayer && Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Limpiar efecto de rastro
        if (trail != null)
        {
            Destroy(trail);
        }

        _bossManager.CanMove = true;
        _bossManager.inMove = false;
    }
}
