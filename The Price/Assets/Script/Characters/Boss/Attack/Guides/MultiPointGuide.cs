using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Guía que muestra múltiples puntos de impacto.
/// Usada para ataques que golpean en múltiples posiciones secuencialmente.
/// </summary>
public class MultiPointGuide : GuideBase
{
    [Header("Multi-Point Settings")]
    [Tooltip("Prefab para cada marcador de punto individual")]
    public GameObject pointMarkerPrefab;

    [Tooltip("Espaciado entre puntos")]
    public float spacing = 1.2f;

    private List<GameObject> _pointMarkers = new List<GameObject>();

    public override void Configure(GuideConfig config)
    {
        if (config is MultiPointGuideConfig multiConfig)
        {
            // Limpiar marcadores previos
            foreach (var marker in _pointMarkers)
            {
                if (marker != null) Destroy(marker);
            }
            _pointMarkers.Clear();

            // Calcular dirección
            Vector3 direction = (multiConfig.target - multiConfig.origin).normalized;

            // Crear marcadores en cada posición
            for (int i = 0; i < multiConfig.pointCount; i++)
            {
                Vector3 pointPos = multiConfig.origin + (direction * (spacing * i));
                GameObject marker = Instantiate(pointMarkerPrefab, pointPos, Quaternion.identity, transform);

                // Configurar color si tiene SpriteRenderer
                SpriteRenderer sr = marker.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = multiConfig.color;
                }

                // Escalar según tamaño
                marker.transform.localScale = Vector3.one * multiConfig.size;

                _pointMarkers.Add(marker);

                // Opcional: retrasar aparición de cada marcador
                if (multiConfig.staggerAppearance)
                {
                    marker.SetActive(false);
                    StartCoroutine(ShowMarkerDelayed(marker, i * multiConfig.staggerDelay));
                }
            }
        }
    }

    private System.Collections.IEnumerator ShowMarkerDelayed(GameObject marker, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (marker != null) marker.SetActive(true);
    }

    private void OnDestroy()
    {
        // Limpiar marcadores al destruir la guía
        foreach (var marker in _pointMarkers)
        {
            if (marker != null) Destroy(marker);
        }
    }
}

/// <summary>
/// Configuración para guías de múltiples puntos
/// </summary>
public class MultiPointGuideConfig : GuideConfig
{
    public int pointCount; // Cantidad de puntos a mostrar
    public bool staggerAppearance; // Si los puntos aparecen secuencialmente
    public float staggerDelay = 0.1f; // Delay entre aparición de puntos
}
