using System.Collections;
using UnityEngine;

/// <summary>
/// Ataque de onda expansiva que se expande desde el boss.
/// Ideal para ataques que empujan o dañan en área creciente.
/// </summary>
public class WaveAttack : AttackBoss
{
    [Header("Wave Data")]
    [Tooltip("Radio máximo de la onda")]
    public float maxWaveRadius = 8f;

    [Tooltip("Velocidad de expansión de la onda")]
    public float expansionSpeed = 5f;

    [Tooltip("Cantidad de ondas a generar")]
    [Range(1, 5)]
    public int waveCount = 1;

    [Tooltip("Delay entre ondas consecutivas")]
    public float delayBetweenWaves = 0.5f;

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            guideInScene = Instantiate(guideObj, enemyParent.transform.position, Quaternion.identity);

            AreaGuide areaGuide = guideInScene.GetComponent<AreaGuide>();
            if (areaGuide != null)
            {
                AreaGuideConfig config = new AreaGuideConfig
                {
                    origin = enemyParent.transform.position,
                    radius = maxWaveRadius
                };
                areaGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override IEnumerator LaunchedAttack()
    {
        // Generar múltiples ondas con delay
        for (int i = 0; i < waveCount; i++)
        {
            ObjectPerDamage wave = Instantiate(visualAttack.gameObject, GetPosition(), Quaternion.identity).GetComponent<ObjectPerDamage>();
            wave.SetValues(GetDamage(), maxWaveRadius / expansionSpeed); // Duración = tiempo para expandirse completamente

            // Iniciar expansión de la onda
            StartCoroutine(ExpandWave(wave.transform, maxWaveRadius));

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    /// <summary>
    /// Expande gradualmente la onda desde el centro
    /// </summary>
    private IEnumerator ExpandWave(Transform wave, float targetRadius)
    {
        float currentRadius = 0f;
        Vector3 startScale = Vector3.zero;

        while (currentRadius < targetRadius)
        {
            currentRadius += expansionSpeed * Time.deltaTime;
            currentRadius = Mathf.Min(currentRadius, targetRadius);

            float diameter = currentRadius * 2;
            wave.localScale = new Vector3(diameter, diameter, 1);

            yield return null;
        }
    }
}
