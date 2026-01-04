using UnityEngine;

/// <summary>
/// Guía de área que muestra un círculo o área de efecto.
/// Usada para explosiones, ataques de área, y efectos radiales.
/// </summary>
public class AreaGuide : GuideBase
{
    [Header("Area Settings")]
    [Tooltip("Si es verdadero, el área se expande desde el centro")]
    public bool expandAnimation;

    [Tooltip("Velocidad de expansión")]
    public float expandSpeed = 1f;

    private float _targetRadius;
    private float _currentRadius;

    public override void Configure(GuideConfig config)
    {
        if (config is AreaGuideConfig areaConfig)
        {
            // Posicionar en el origen/centro del ataque
            transform.position = areaConfig.origin;

            _targetRadius = areaConfig.radius;

            if (expandAnimation)
            {
                _currentRadius = 0;
            }
            else
            {
                _currentRadius = _targetRadius;
            }

            UpdateVisuals();

            if (spriteRenderer != null)
            {
                spriteRenderer.color = areaConfig.color;
            }
        }
    }

    private void Update()
    {
        if (expandAnimation && _currentRadius < _targetRadius)
        {
            _currentRadius += expandSpeed * Time.deltaTime;
            _currentRadius = Mathf.Min(_currentRadius, _targetRadius);
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (visualTransform != null)
        {
            float diameter = _currentRadius * 2;
            visualTransform.localScale = new Vector3(diameter, diameter, 1);
        }
    }
}

/// <summary>
/// Configuración para guías de área
/// </summary>
public class AreaGuideConfig : GuideConfig
{
    public float radius; // Radio del área de efecto
}
