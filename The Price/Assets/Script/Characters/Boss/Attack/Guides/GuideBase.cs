using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los tipos de guías visuales de ataques de boss.
/// Las guías muestran al jugador dónde ocurrirá un ataque antes de que se lance.
/// </summary>
public abstract class GuideBase : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("Transform del elemento visual que se escalará/rotará")]
    public Transform visualTransform;

    [Tooltip("Color de la guía (opcional, se puede animar)")]
    public SpriteRenderer spriteRenderer;

    /// <summary>
    /// Configura la guía con los parámetros específicos del ataque.
    /// Cada tipo de guía implementa su propia lógica de visualización.
    /// </summary>
    public abstract void Configure(GuideConfig config);

    /// <summary>
    /// Llamado opcionalmente para actualizar la guía durante su tiempo de vida.
    /// Útil para guías que necesitan seguir al jugador o animarse.
    /// </summary>
    public virtual void UpdateGuide() { }
}

/// <summary>
/// Configuración base para todas las guías.
/// Se extiende con configuraciones específicas para cada tipo de guía.
/// </summary>
public class GuideConfig
{
    public Vector3 origin;
    public Vector3 target;
    public float size;
    public float duration;
    public Color color = Color.white;
}
