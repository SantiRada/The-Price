using UnityEngine;

/// <summary>
/// Guía de punto que muestra un marcador circular/cuadrado en una posición específica.
/// Usada para ataques que caen desde arriba o aparecen en una posición fija.
/// </summary>
public class PointGuide : GuideBase
{
    [Header("Point Settings")]
    [Tooltip("Si es verdadero, el tamaño de la guía pulsa/anima")]
    public bool pulseAnimation;

    [Tooltip("Velocidad de la animación de pulso")]
    public float pulseSpeed = 2f;

    private float _baseScale;
    private float _time;

    public override void Configure(GuideConfig config)
    {
        if (config is PointGuideConfig pointConfig)
        {
            // Posicionar en el objetivo
            transform.position = pointConfig.target;

            // Escalar según el tamaño del ataque
            if (visualTransform != null)
            {
                visualTransform.localScale = Vector3.one * pointConfig.size;
                _baseScale = pointConfig.size;
            }

            // Rotar si se especifica
            if (pointConfig.rotation != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, pointConfig.rotation);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = pointConfig.color;
            }
        }
    }

    private void Update()
    {
        if (pulseAnimation && visualTransform != null)
        {
            _time += Time.deltaTime * pulseSpeed;
            float scale = _baseScale + (Mathf.Sin(_time) * 0.1f * _baseScale);
            visualTransform.localScale = Vector3.one * scale;
        }
    }
}

/// <summary>
/// Configuración para guías de punto
/// </summary>
public class PointGuideConfig : GuideConfig
{
    public float rotation; // Rotación en grados Z
}
