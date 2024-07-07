using UnityEngine;

public class GuideProjectile : MonoBehaviour {

    public Transform visualGuide;

    public void SetSize(Vector3 target, float size, bool changeSize = false)
    {
        // Calcula la direcci�n hacia el objetivo
        Vector3 direction = target - transform.position;

        // Calcula el �ngulo en radianes y luego convierte a grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica la rotaci�n
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        if (changeSize)
        {
            visualGuide.localPosition = new Vector3((size / 2), visualGuide.localPosition.y, visualGuide.localPosition.z);
            visualGuide.localScale = new Vector3(size, visualGuide.localScale.y, visualGuide.localScale.z);
        }
    }
}
