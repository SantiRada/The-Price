using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [Header("State")]
    public float delayToLessHealth;
    public float speedMoveChanges;
    private bool healthChanges = false;
    private float delayBaseHealth;

    [Header("Content UI")]
    public Image delayBar;
    [Space]
    public Image healthbar;
    public Image healthFeedback;
    [Space]
    public Image concenBar;
    public Image concenFeedback;
    [Space]
    public Image[] skills;

    private void Start()
    {
        delayBaseHealth = delayToLessHealth;

        for (int i = 0; i < skills.Length; i++) { if (skills[i].sprite == null) { skills[i].gameObject.SetActive(false); } }
    }
    private void Update()
    {
        if (!healthChanges) return;

        delayToLessHealth -= Time.deltaTime;
        delayBar.fillAmount = delayToLessHealth / delayBaseHealth;

        if (delayToLessHealth <= 0) { StartCoroutine("UpdateHealthFeedback"); }
    }
    public void SetHealthbar(float health, float healthMax)
    {
        healthbar.fillAmount = health / healthMax;

        healthChanges = true;
    }
    public void SetConcentracion(float concentracion, float concentracionMax)
    {
        concenBar.fillAmount = concentracion / concentracionMax;
        StartCoroutine("UpdateConcentracionFeedback");
    }
    public void SetSkills(int pos, Sprite spr)
    {
        skills[pos].gameObject.SetActive(true);

        skills[pos].sprite = spr;
    }
    // ---- SETTERS && GETTERS ---- //
    public void HealPerRoom(PlayerStats stats)
    {
        // RESET = INFORMACION DEL DELAY
        healthChanges = false;
        delayToLessHealth = delayBaseHealth;
        delayBar.fillAmount = 1;

        if (healthFeedback.fillAmount != healthbar.fillAmount)
        {
            float diff = (healthFeedback.fillAmount - healthbar.fillAmount) * stats.GetterStats(0, true);

            stats.SetValue(0, (int)diff, false);
            StartCoroutine("HealHealthbarBasePerRoom");
        }
    }
    private IEnumerator HealHealthbarBasePerRoom()
    {
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < 16; i++)
        {
            healthbar.fillAmount = Mathf.Lerp(healthbar.fillAmount, healthFeedback.fillAmount, speedMoveChanges * Time.deltaTime);
            yield return null;
        }

        healthbar.fillAmount = healthFeedback.fillAmount;
    }
    // ---- FEEDBACK LENTO ---- //
    private IEnumerator UpdateHealthFeedback()
    {
        healthChanges = false;
        delayToLessHealth = delayBaseHealth;
        delayBar.fillAmount = 1;

        for (int i = 0; i < 16; i++)
        {
            healthFeedback.fillAmount = Mathf.Lerp(healthFeedback.fillAmount, healthbar.fillAmount, speedMoveChanges * Time.deltaTime);
            yield return null;
        }

        healthFeedback.fillAmount = healthbar.fillAmount;
    }
    private IEnumerator UpdateConcentracionFeedback()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 16; i++)
        {
            concenFeedback.fillAmount = Mathf.Lerp(concenFeedback.fillAmount, concenBar.fillAmount, speedMoveChanges * Time.deltaTime);
            yield return null;
        }
        concenFeedback.fillAmount = concenBar.fillAmount;
    }
}