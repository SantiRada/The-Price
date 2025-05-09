using UnityEngine;

public class GuideProjectile : MonoBehaviour {

    [Tooltip("Referencia visual del proyectil guía que se escala y posiciona visualmente según el tamaño")]
    public Transform visualGuide;

    public void SetSize(Vector3 target, float size, bool changeSize = false)
    {
        // Calcula la dirección hacia el objetivo
        Vector3 direction = target - transform.position;

        // Calcula el ángulo en radianes y luego convierte a grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica la rotación del objeto hacia el objetivo
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (changeSize)
        {
            // Reposiciona y escala la guía visual para ajustarse al tamaño deseado
            visualGuide.localPosition = new Vector3((size / 2), visualGuide.localPosition.y, visualGuide.localPosition.z);
            visualGuide.localScale = new Vector3(size, visualGuide.localScale.y, visualGuide.localScale.z);
        }
    }
}
