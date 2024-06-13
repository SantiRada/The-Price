using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {

    public Image healthbar;
    public Image shieldbar;

    private GameObject target;
    private Vector2 offset;

    private void Start() { healthbar.fillAmount = 1; }
    public void SetInitialValues(GameObject tg, Vector2 off)
    {
        target = tg;
        offset = off;
    }
    private void Update() { transform.position = ((Vector2)target.transform.position + offset); }
    public void SetHealthbar(float healthMax, float health, float shieldMax, float shield)
    {
        healthbar.fillAmount = (health / healthMax);
        shieldbar.fillAmount = (shield / shieldMax);
    }
}
