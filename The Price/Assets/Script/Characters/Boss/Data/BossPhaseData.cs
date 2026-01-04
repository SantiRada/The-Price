using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Datos de configuración para una fase específica de un boss.
/// Permite definir ataques, movimientos, y cambios de stats por fase.
/// </summary>
[System.Serializable]
public class BossPhaseData
{
    [Header("Phase Trigger")]
    [Tooltip("Umbral para cambiar a esta fase (vida restante o tiempo transcurrido)")]
    public int triggerValue;

    [Header("Stats Multipliers")]
    [Tooltip("Multiplicador de daño para esta fase (1.0 = sin cambios)")]
    [Range(0.5f, 3f)]
    public float damageMultiplier = 1f;

    [Tooltip("Multiplicador de velocidad de movimiento (1.0 = sin cambios)")]
    [Range(0.5f, 2f)]
    public float speedMultiplier = 1f;

    [Tooltip("Multiplicador de velocidad de ataque (1.0 = sin cambios)")]
    [Range(0.5f, 2f)]
    public float attackSpeedMultiplier = 1f;

    [Header("Combat Behavior")]
    [Tooltip("Cantidad de ataques diferentes disponibles en esta fase")]
    public int attackCount;

    [Tooltip("Si es verdadero, incluye todos los ataques de fases anteriores")]
    public bool includeAllPreviousAttacks = false;

    [Tooltip("Probabilidad de atacar vs moverse (0-100)")]
    [Range(0, 100)]
    public int aggressionLevel = 50;

    [Tooltip("Tiempo mínimo entre ataques en segundos")]
    public float minAttackCooldown = 1.5f;

    [Tooltip("Tiempo máximo entre ataques en segundos")]
    public float maxAttackCooldown = 3f;

    [Header("Visual Feedback")]
    [Tooltip("Color de la barra de vida en esta fase")]
    public Color phaseColor = Color.red;

    [Tooltip("Efecto visual al cambiar a esta fase (opcional)")]
    public GameObject phaseChangeEffect;

    [Header("Movement Patterns")]
    [Tooltip("Patrones de movimiento habilitados en esta fase")]
    public List<int> enabledMovementPatterns = new List<int>();
}
