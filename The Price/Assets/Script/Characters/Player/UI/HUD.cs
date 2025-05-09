using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [Header("State")]
    public float delayToLessHealth;
    public float speedMoveChanges;
    private bool healthChanges = false;
    private float delayBaseHealth;
    [Space]
    private int countGold = 0;
    [HideInInspector] public int countFinishGold = 0;
    [Space]
    private int countSouls = 0;
    [HideInInspector] public int countFinishSouls;

    [Header("Bars")]
    public TextMeshProUGUI textHealth;
    public Image healthbar;
    public Image healthFeedback;
    [Space]
    public Image concenBar;
    public Image concenFeedback;
    [Space]
    public CanvasGroup canvasDelay;
    public Image delayBar;

    [Header("Gold")]
    public TextMeshProUGUI goldText;

    [Header("Souls")]
    public GameObject sectorSouls;
    public TextMeshProUGUI soulsText;

    [Header("Weapon & Skills")]
    public Image[] weapons;
    public Image[] skills;

    [Header("Private Content")]
    private PlayerStats _player;
    private DeadSystem _deadSystem;

    private static int _staticCountGold;
    private static int _staticCountSouls;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerStats>();
        _deadSystem = _player.GetComponent<DeadSystem>();
    }
    private void Start()
    {
        canvasDelay.alpha = 0;
        delayBaseHealth = delayToLessHealth;
        countFinishSouls = countSouls;

        goldText.text = countGold.ToString();
        soulsText.text = countSouls.ToString();
        textHealth.text = _player.GetterStats(0, false).ToString() + "/" + _player.GetterStats(0, true).ToString();

        if (_deadSystem.canHaveSouls) sectorSouls.gameObject.SetActive(true);
        else sectorSouls.gameObject.SetActive(false);

        for (int i = 0; i < skills.Length; i++) { if (skills[i].sprite == null) { skills[i].gameObject.SetActive(false); } }
        for (int i = 0; i < weapons.Length; i++) { if (weapons[i].sprite == null) { weapons[i].gameObject.SetActive(false); } }
    }
    private void Update()
    {
        if (!healthChanges) { return; }

        delayToLessHealth -= Time.deltaTime;
        delayBar.fillAmount = delayToLessHealth / delayBaseHealth;

        if (delayToLessHealth <= 0) { StartCoroutine("UpdateHealthFeedback"); }

        _staticCountGold = countFinishGold;
        _staticCountSouls = countFinishSouls;
    }
    private IEnumerator IncreaseGold()
    {
        yield return new WaitForSeconds(0.25f);

        while (countGold < countFinishGold)
        {
            countGold++;
            goldText.text = countGold.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator IncreaseSouls()
    {
        yield return new WaitForSeconds(0.25f);

        while (countSouls < countFinishSouls)
        {
            countSouls++;
            soulsText.text = countSouls.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
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

            textHealth.text = stats.GetterStats(0, false).ToString() + "/" + stats.GetterStats(0, true).ToString();
            StartCoroutine("HealHealthbarBasePerRoom");
        }
    }
    // ---- SETTERS ---- //
    public void ShowSouls() { sectorSouls.gameObject.SetActive(true); }
    public void SetSouls(int souls)
    {
        countFinishSouls += souls;

        StartCoroutine("IncreaseSouls");
    }
    public void SetGold(int gold)
    {
        countFinishGold += gold;

        StartCoroutine("IncreaseGold");
    }
    public void SetHealthbar(float health, float healthMax)
    {
        healthbar.fillAmount = health / healthMax;

        if(health > 0) textHealth.text = health.ToString() + "/" + healthMax.ToString();
        else textHealth.text = "0/" + healthMax.ToString();

        if (health != healthMax)
        {
            healthChanges = true;
            canvasDelay.alpha = 1;
        }
    }
    public void SetConcentracion(float concentracion, float concentracionMax)
    {
        concenBar.fillAmount = concentracion / concentracionMax;
        StartCoroutine("UpdateConcentracionFeedback");
    }
    public void SetWeapon(int pos, Sprite spr)
    {
        weapons[pos].gameObject.SetActive(true);

        weapons[pos].sprite = spr;
    }
    public void SetSkills(int pos, Sprite spr)
    {
        skills[pos].gameObject.SetActive(true);

        skills[pos].sprite = spr;
    }
    // ---- SETTERS && GETTERS ---- //
    public int GetGold() { return countFinishGold; }
    public int GetSouls() { return countSouls; }
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
        canvasDelay.alpha = 0;
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
    // ---- STATIC ---- //
    public static int GetCountGold() { return _staticCountGold; }
    public static int  GetCountSouls() { return _staticCountSouls; }
}