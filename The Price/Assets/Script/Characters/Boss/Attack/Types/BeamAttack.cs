using System.Collections;
using UnityEngine;

/// <summary>
/// Ataque de rayo continuo que apunta hacia el jugador.
/// El rayo permanece activo durante un tiempo y puede seguir al jugador.
/// </summary>
public class BeamAttack : AttackBoss
{
    [Header("Beam Data")]
    [Tooltip("Longitud máxima del rayo")]
    public float beamLength = 15f;

    [Tooltip("Ancho del rayo")]
    public float beamWidth = 0.5f;

    [Tooltip("Duración del rayo activo")]
    public float beamDuration = 2f;

    [Tooltip("Si es verdadero, el rayo sigue al jugador mientras está activo")]
    public bool followPlayer = true;

    [Tooltip("Velocidad de rotación del rayo (si sigue al jugador)")]
    public float rotationSpeed = 180f;

    protected override void CreateGuide()
    {
        if (guideObj != null)
        {
            guideInScene = Instantiate(guideObj, enemyParent.transform.position, Quaternion.identity);

            LinearGuide linearGuide = guideInScene.GetComponent<LinearGuide>();
            if (linearGuide != null)
            {
                LinearGuideConfig config = new LinearGuideConfig
                {
                    origin = enemyParent.transform.position,
                    target = GetPlayerPosition(),
                    size = beamLength,
                    targetTransform = followPlayer ? _player.transform : null
                };
                linearGuide.Configure(config);
            }

            Destroy(guideInScene, timeToGuide);
        }
    }

    protected override Vector3 GetPosition() { return enemyParent.transform.position; }

    protected override IEnumerator LaunchedAttack()
    {
        Vector3 startPos = GetPosition();
        Vector3 direction = (GetPlayerPosition() - startPos).normalized;

        // Crear el rayo
        ObjectPerDamage beam = Instantiate(visualAttack.gameObject, startPos, Quaternion.identity).GetComponent<ObjectPerDamage>();
        beam.SetValues(GetDamage(), beamDuration);

        // Configurar escala del rayo
        beam.transform.localScale = new Vector3(beamLength, beamWidth, 1);

        // Rotar hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        beam.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Si el rayo debe seguir al jugador, actualizar su rotación
        if (followPlayer)
        {
            StartCoroutine(UpdateBeamDirection(beam.transform, beamDuration));
        }

        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Actualiza la dirección del rayo para seguir al jugador
    /// </summary>
    private IEnumerator UpdateBeamDirection(Transform beam, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && beam != null)
        {
            Vector3 direction = (_player.transform.position - beam.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            beam.rotation = Quaternion.RotateTowards(beam.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
