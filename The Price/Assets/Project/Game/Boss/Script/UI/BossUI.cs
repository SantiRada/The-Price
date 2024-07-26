using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour {

    [Header("UI Content")]
    public TextMeshProUGUI txtNameBoss;
    public Slider healthbar;
    public Slider shieldbar;

    [Header("Stats")]
    [Range(0, 0.2f)] public float timeBetweenLetters;
    [Range(0, 0.2f)] public float timeBetweenLoadBars;
    private int health;
    private int shield;
    private string nameBoss;

    [Header("Private Content")]
    private CanvasGroup _canvas;

    private void Start()
    {
        _canvas = GetComponent<CanvasGroup>();

        InitialValues();
    }
    // ---- CALLERS PER BOSS ---- //
    public void StartUIPerBoss(int txt, int health, int shield)
    {
        nameBoss = LanguageManager.GetValue("Game", txt);
        this.health = health;
        this.shield = shield;

        StartCoroutine("StartUI");
    }
    public void SetStats(int health, int shield)
    {
        StartCoroutine(SetNewStats(health, shield));
    }
    public IEnumerator HideUI()
    {
        do
        {
            _canvas.alpha -= 0.1f;
            yield return new WaitForSeconds(0.05f); // OneSecond
        } while (_canvas.alpha < 1);

        InitialValues();
    }
    // ---- FUNCION INTEGRA ---- //
    private IEnumerator StartUI()
    {
        do
        {
            _canvas.alpha += 0.1f;
            yield return new WaitForSeconds(0.05f); // OneSecond
        } while (_canvas.alpha < 1);

        StartCoroutine("SetInitialStats");

        for(int i = 0; i < nameBoss.Length; i++)
        {
            txtNameBoss.text += nameBoss[i];
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }
    private IEnumerator SetInitialStats()
    {
        healthbar.maxValue = health;

        float between = health / 4;

        for(int i = 0; i < health; i++)
        {
            healthbar.value += 1;
            yield return new WaitForSeconds(between);
        }

        shieldbar.maxValue = shield;
        for (int i = 0; i < shield; i++)
        {
            shieldbar.value += 1;
            yield return new WaitForSeconds(between);
        }
    }
    private IEnumerator SetNewStats(int health, int shield)
    {
        this.health = health;
        this.shield = shield;

        if(shield > 0)
        {
            for(int i = 0; i < (shieldbar.value - shield); i++)
            {
                shieldbar.value -= 1;
                yield return new WaitForSeconds(0.05f);
            }
        }

        for (int i = 0; i < (healthbar.value - health); i++)
        {
            healthbar.value -= 1;
            yield return new WaitForSeconds(0.05f);
        }

        healthbar.value = health;
        shieldbar.value = shield;
    }
    private void InitialValues()
    {
        _canvas.alpha = 0;
        _canvas.interactable = false;
        txtNameBoss.text = "";
        healthbar.value = 0;
        shieldbar.value = 0;
    }
}
