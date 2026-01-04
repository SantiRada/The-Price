using UnityEngine;

/// <summary>
/// Guía lineal que muestra una línea desde un origen hasta un objetivo.
/// Usada para proyectiles, ataques de suelo múltiples, y beams direccionales.
/// </summary>
public class LinearGuide : GuideBase
{
    [Header("Linear Settings")]
    [Tooltip("Si es verdadero, la guía sigue al objetivo durante su duración")]
    public bool followTarget;

    private Vector3 _currentTarget;
    private Transform _targetTransform;

    public override void Configure(GuideConfig config)
    {
        if (config is LinearGuideConfig linearConfig)
        {
            transform.position = linearConfig.origin;
            _currentTarget = linearConfig.target;

            if (linearConfig.targetTransform != null && followTarget)
            {
                _targetTransform = linearConfig.targetTransform;
            }

            UpdateVisuals(linearConfig.origin, _currentTarget, linearConfig.size);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = linearConfig.color;
            }
        }
    }

    public override void UpdateGuide()
    {
        if (followTarget && _targetTransform != null)
        {
            _currentTarget = _targetTransform.position;
            UpdateVisuals(transform.position, _currentTarget, 0);
        }
    }

    private void UpdateVisuals(Vector3 origin, Vector3 target, float distance)
    {
        // Calcular dirección y rotación
        Vector3 direction = target - origin;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Si tiene visualTransform, escalarlo para cubrir la distancia
        if (visualTransform != null && distance > 0)
        {
            float actualDistance = direction.magnitude;
            visualTransform.localPosition = new Vector3(actualDistance / 2, 0, 0);
            visualTransform.localScale = new Vector3(actualDistance, visualTransform.localScale.y, visualTransform.localScale.z);
        }
    }

    private void Update()
    {
        if (followTarget)
        {
            UpdateGuide();
        }
    }
}

/// <summary>
/// Configuración para guías lineales
/// </summary>
public class LinearGuideConfig : GuideConfig
{
    public Transform targetTransform; // Opcional: para seguir al jugador
    public bool showMultiplePoints; // Para ataques de suelo múltiples
    public int pointCount; // Cantidad de puntos a mostrar
}
