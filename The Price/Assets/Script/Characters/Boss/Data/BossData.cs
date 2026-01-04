using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject que define la configuración completa de un boss.
/// Permite crear y modificar bosses sin tocar código, solo configurando datos.
/// </summary>
[CreateAssetMenu(fileName = "NewBoss", menuName = "Boss/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Boss Identity")]
    [Tooltip("Nombre único del boss para el sistema de guardado")]
    public int bossID;

    [Tooltip("Nombre display del boss")]
    public string bossName;

    [Tooltip("Tipo de boss (miniboss, boss, maxboss)")]
    public TypeBoss bossType;

    [Header("Base Stats")]
    [Tooltip("Vida máxima del boss")]
    public int maxHealth = 100;

    [Tooltip("Escudo inicial del boss")]
    public int initialShield = 0;

    [Tooltip("Daño base de los ataques")]
    public int baseDamage = 10;

    [Tooltip("Velocidad de movimiento base")]
    public float baseSpeed = 3f;

    [Tooltip("Defensa del boss (reduce daño recibido)")]
    [Range(0, 50)]
    public int defense = 0;

    [Header("Phase System")]
    [Tooltip("Si es verdadero, cambia de fase por vida. Si es falso, por tiempo.")]
    public bool changePhaseByHealth = true;

    [Tooltip("Configuración de cada fase del boss")]
    public List<BossPhaseData> phases = new List<BossPhaseData>();

    [Header("Attacks")]
    [Tooltip("Prefabs de todos los ataques disponibles para este boss")]
    public List<GameObject> attackPrefabs = new List<GameObject>();

    [Header("Movement")]
    [Tooltip("Prefabs de patrones de movimiento disponibles")]
    public List<GameObject> movementPatternPrefabs = new List<GameObject>();

    [Tooltip("Distancia máxima de combate (si el jugador está más lejos, el boss se mueve)")]
    public float maxCombatDistance = 10f;

    [Tooltip("Distancia mínima de combate (si el jugador está más cerca, el boss retrocede)")]
    public float minCombatDistance = 2f;

    [Header("Resistances & Immunities")]
    [Tooltip("Elementos a los que el boss es inmune")]
    public List<string> immunities = new List<string>();

    [Tooltip("Elementos que causan daño reducido (50%)")]
    public List<string> resistances = new List<string>();

    [Tooltip("Elementos que causan daño aumentado (150%)")]
    public List<string> weaknesses = new List<string>();

    [Header("Visual & Audio")]
    [Tooltip("Prefab del boss (debe tener componente BossSystem)")]
    public GameObject bossPrefab;

    [Tooltip("Sprite o modelo del boss")]
    public Sprite bossSprite;

    [Tooltip("Música de combate específica para este boss (opcional)")]
    public AudioClip bossCombatMusic;

    [Header("Rewards")]
    [Tooltip("Cantidad de oro que suelta al morir")]
    public CountGold goldReward = CountGold.medium;

    [Tooltip("Probabilidad de soltar objetos especiales (0-100)")]
    [Range(0, 100)]
    public int specialDropChance = 20;

    [Header("Advanced Settings")]
    [Tooltip("Tiempo de presentación del boss en segundos")]
    public float presentationDuration = 3.5f;

    [Tooltip("Tiempo de animación de muerte en segundos")]
    public float deathDuration = 3.5f;

    [Tooltip("Si es verdadero, el boss puede moverse mientras ataca")]
    public bool canMoveWhileAttacking = false;

    /// <summary>
    /// Obtiene la configuración de una fase específica
    /// </summary>
    public BossPhaseData GetPhaseData(int phaseIndex)
    {
        if (phaseIndex >= 0 && phaseIndex < phases.Count)
        {
            return phases[phaseIndex];
        }
        return null;
    }

    /// <summary>
    /// Obtiene el número total de fases
    /// </summary>
    public int GetPhaseCount()
    {
        return phases.Count;
    }

    /// <summary>
    /// Verifica si el boss es inmune a un elemento
    /// </summary>
    public bool IsImmuneTo(string element)
    {
        return immunities.Contains(element);
    }

    /// <summary>
    /// Obtiene el multiplicador de daño para un elemento específico
    /// </summary>
    public float GetElementalMultiplier(string element)
    {
        if (immunities.Contains(element)) return 0f;
        if (resistances.Contains(element)) return 0.5f;
        if (weaknesses.Contains(element)) return 1.5f;
        return 1f;
    }
}
