using UnityEngine;

public class GuideProjectile : MonoBehaviour {

    public Transform visualGuide;

    public void SetSize(Vector3 target, float size)
    {
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        visualGuide.localPosition = new Vector3((size / 2), visualGuide.localPosition.y, visualGuide.localPosition.z);
        visualGuide.localScale = new Vector3(size, visualGuide.localScale.y, visualGuide.localScale.z);
    }
}
