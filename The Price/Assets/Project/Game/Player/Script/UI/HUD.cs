using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [Header("Content UI")]
    public Image healthbar;
    public Image concenBar;
    public Image[] skills;

    private void Start()
    {
        healthbar.fillAmount = 1;
        for(int i = 0; i < skills.Length; i++)
        {
            if (skills[i].sprite == null) { skills[i].gameObject.SetActive(false); }
        }
    }
    public void SetHealthbar(float health, float healthMax) { healthbar.fillAmount = health / healthMax; }
    public void SetConcentracion(float concentracion, float concentracionMax) { concenBar.fillAmount = concentracion / concentracionMax ; }
    public void SetSkills(int pos, Sprite spr)
    {
        skills[pos].gameObject.SetActive(true);

        skills[pos].sprite = spr;
    }
}