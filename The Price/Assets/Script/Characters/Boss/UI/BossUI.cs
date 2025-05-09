using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour {

    [Header("UI Content")]
    [Tooltip("Texto que muestra el nombre del jefe en pantalla")]
    public TextMeshProUGUI txtNameBoss;

    [Tooltip("Barra de vida del jefe")]
    public Slider healthbar;

    [Tooltip("Barra de escudo del jefe")]
    public Slider shieldbar;

    [Header("Stats")]
    [Tooltip("Tiempo entre cada letra al mostrar el nombre del jefe")]
    [Range(0, 0.2f)] public float timeBetweenLetters;

    [Tooltip("Tiempo entre actualizaciones visuales de las barras")]
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
        // Muestra progresivamente la UI y luego escribe el nombre letra por letra
        do
        {
            _canvas.alpha += 0.1f;
            yield return new WaitForSeconds(0.05f); // OneSecond
        } while (_canvas.alpha < 1);

        StartCoroutine("SetInitialStats");

        for (int i = 0; i < nameBoss.Length; i++)
        {
            txtNameBoss.text += nameBoss[i];
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }

    private IEnumerator SetInitialStats()
    {
        healthbar.maxValue = health;
        shieldbar.maxValue = shield;

        // Anima el llenado inicial de las barras de vida y escudo
        for (int i = 0; i < health; i++)
        {
            healthbar.value += 1;
            yield return new WaitForSeconds(0.0025f);
        }

        for (int i = 0; i < shield; i++)
        {
            shieldbar.value += 1;
            yield return new WaitForSeconds(0.0025f);
        }
    }

    private IEnumerator SetNewStats(int health, int shield)
    {
        this.health = health;
        this.shield = shield;

        // Reduce el valor actual de escudo visualmente si es necesario
        if (shield > 0)
        {
            for (int i = 0; i < (shieldbar.value - shield); i++)
            {
                shieldbar.value -= 1;
                yield return new WaitForSeconds(0.05f);
            }
        }

        // Reduce el valor actual de vida visualmente si es necesario
        for (int i = 0; i < (healthbar.value - health); i++)
        {
            healthbar.value -= 1;
            yield return new WaitForSeconds(0.05f);
        }

        // Asigna el valor final corregido por seguridad
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
