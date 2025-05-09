using UnityEngine;

public class GuideProjectile : MonoBehaviour {

    [Tooltip("Referencia visual del proyectil gu�a que se escala y posiciona visualmente seg�n el tama�o")]
    public Transform visualGuide;

    public void SetSize(Vector3 target, float size, bool changeSize = false)
    {
        // Calcula la direcci�n hacia el objetivo
        Vector3 direction = target - transform.position;

        // Calcula el �ngulo en radianes y luego convierte a grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica la rotaci�n del objeto hacia el objetivo
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (changeSize)
        {
            // Reposiciona y escala la gu�a visual para ajustarse al tama�o deseado
            visualGuide.localPosition = new Vector3((size / 2), visualGuide.localPosition.y, visualGuide.localPosition.z);
            visualGuide.localScale = new Vector3(size, visualGuide.localScale.y, visualGuide.localScale.z);
        }
    }
}
