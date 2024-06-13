using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public enum TypeFlair { PV, Concentracion, VelocidadMovimiento, VelocidadAtaque, SkillDamage, Damage, DamageSubsequence, CriticalChance, MissChance, StealingPV, sanity }
public class FlairSystem : MonoBehaviour {

    [Header("Content Flair")]
    public Sprite[] iconPerFlair;

    [Header("Content UI")]
    public Image[] icons;
    public TextMeshProUGUI[] titleFlair;
    public TextMeshProUGUI[] descFlair;
    public TextMeshProUGUI[] dataFlair;
    public TextMeshProUGUI[] dataAffectedFlair;
    private CanvasGroup canvas;

    [Header("Movement")]
    public Color selectedColor;
    public Color unselectedColor;
    public Image[] contentUI;
    private int index = 0;
    private int prevIndex = 0;
    private bool canMove = false;

    [SerializeField] private List<TypeFlair> types = new List<TypeFlair>();
    [SerializeField] private List<TypeFlair> typesAffected = new List<TypeFlair>();
    private List<int> amountPerType = new List<int>();

    private static bool startFlair = false;
    private PlayerStats _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerStats>();

        canvas = GetComponent<CanvasGroup>();
    }
    private void Start() { ResetValues(); }
    public static void StartFlairSelector()
    {
        startFlair = true;
    }
    private void Update()
    {
        if (LoadingScreen.inLoading) return;

        if (startFlair) StartCoroutine("FlairSelector");

        if (!canMove) return;

        float v = Input.GetAxisRaw("Vertical");
        if (v != 0) StartCoroutine(MoveInUI(v));

        if (Input.GetButtonDown("Fire1") || PlayerActionStates.IsUse) StartCoroutine("Select");
    }
    private IEnumerator FlairSelector()
    {
        startFlair = false;
        ActionForControlPlayer.ChangeDetectClic(false);
        Pause.StateChange = State.Interface;

        //  CALCULAR VALORES A MOSTRAR
        for(int i = 0; i < 3; i++)
        {
            types.Add(RandomFlairInSelector());
            amountPerType.Add(CalculateAmount());

            typesAffected.Add(RandomAffectedFlair(i));
        }

        // CARGAR LA INFORMACIÓN DE LA UI
        for(int i = 0; i< 3; i++)
        {
            icons[i].sprite = iconPerFlair[(int)types[i]];

            titleFlair[i].text = LanguageManager.GetValue("Game", 25) + " " + LanguageManager.GetValue("Game", (26 + (int)types[i]));

            // INFORMACIÓN DE LA DESCRIPCIÓN DE CADA INSTINTO
            descFlair[i].text = ((int)types[i] != 8) ? LanguageManager.GetValue("Game", 37) : LanguageManager.GetValue("Game", 38);
            descFlair[i].text += " <color=#" + FloatTextManager.GetColor(types[i]).ToHexString() + ">" + LanguageManager.GetValue("Game", (2 + (int)types[i])) + "</color> ";
            descFlair[i].text += LanguageManager.GetValue("Game", 39) + " <color=green>" + (((int)types[i] != 8) ? "+" : "-") + amountPerType[i] + "</color> \n";

            descFlair[i].text += ((int)typesAffected[i] != 8) ? LanguageManager.GetValue("Game", 38) : LanguageManager.GetValue("Game", 37);
            descFlair[i].text += " <color=#" + FloatTextManager.GetColor(typesAffected[i]).ToHexString() + ">" + LanguageManager.GetValue("Game", (2 + (int)typesAffected[i])) + "</color> ";
            descFlair[i].text += LanguageManager.GetValue("Game", 39) + " <color=red>" + (((int)typesAffected[i] != 8) ? "-" : "+") + (int)(amountPerType[i] / 2) + "</color>";

            dataFlair[i].text = LanguageManager.GetValue("Game", (2 + (int)types[i])) + " <color=green>" + _player.GetterStats((int)types[i], true) + " >> " + CalculateNewValue(types[i], amountPerType[i], true) + "</color>";
            dataAffectedFlair[i].text = LanguageManager.GetValue("Game", (2 + (int)typesAffected[i])) + " <color=red>" + _player.GetterStats((int)typesAffected[i], true) + " > " + CalculateNewValue(typesAffected[i], (int)(amountPerType[i] / 2), false) + "</color>";
        }

        yield return new WaitForSeconds(0.1f);

        // HACER VISIBLE LA SECCIÓN DE SELECCIÓN DE INSTINTOS
        canvas.interactable = true;
        canvas.alpha = 1;

        // ACOMODA EL POSICIONAMIENTO DEL JOYSTICK ANTES DE ENCENDERLO
        index = 0;
        prevIndex = 0;
        canMove = true;

        yield return new WaitForSeconds(0.19f);

        ActionForControlPlayer.ChangeDetectClic(true);
    }
    private IEnumerator MoveInUI(float dir)
    {
        canMove = false;
        // MANEJO DE MOVILIDAD BÁSICO POR LA UI
        if (dir > 0) index--;
        else index++;
        // VALORES MÁXIMOS Y MÍNIMOS
        if (index < 0) index = 2;
        else if (index > 2) index = 0;
        // ---------------------------------- //

        contentUI[prevIndex].color = unselectedColor;
        contentUI[index].color = selectedColor;

        prevIndex = index;

        yield return new WaitForSeconds(0.2f);

        canMove = true;
    }
    private IEnumerator Select()
    {
        canMove = false;

        // NUEVOS VALORES BASE
        _player.SetValue((int)types[index], amountPerType[index], false, false);
        _player.SetValue((int)types[index], amountPerType[index], true);

        // NUEVOS VALORES AFECTADOS
        _player.SetValue((int)typesAffected[index], -(int)(amountPerType[index] / 2), false, false);
        _player.SetValue((int)typesAffected[index], -(int)(amountPerType[index] / 2), true);

        yield return new WaitForSeconds(0.25f);

        ResetValues();
    }
    // ---- FUNCIÓN INTEGRA ---- //
    private void ResetValues()
    {
        startFlair = false;
        canvas.alpha = 0;
        canvas.interactable = false;
        index = 0;
        prevIndex = 0;

        Pause.StateChange = State.Game;

        types.Clear();
        typesAffected.Clear();
        amountPerType.Clear();

        for(int i = 0; i < 3; i++)
        {
            icons[i].sprite = null;
            titleFlair[i].text = "";
            descFlair[i].text = "";
            dataFlair[i].text = "";
            dataAffectedFlair[i].text = "";

            contentUI[i].color = unselectedColor;
        }
    }
    private TypeFlair RandomAffectedFlair(int i)
    {
        TypeFlair flair;
        bool canUse = false;

        do
        {
            flair = (TypeFlair)Random.Range(0, 11);

            if (flair != types[i] && _player.GetterStats((int)flair, true) > amountPerType[i]) canUse = true;
        } while (!canUse);

        return flair;
    }
    private TypeFlair RandomFlairInSelector()
    {
        TypeFlair flair;
        bool canUse = true;

        do
        {
            flair = (TypeFlair)Random.Range(0, 11);

            for (int i = 0; i < types.Count; i++)
            {
                if (flair == types[i])
                {
                    canUse = false;
                    break;
                }

                if (flair != types[i] && i >= (types.Count - 1)) canUse = true;
            }

        } while (!canUse);

        return flair;
    }
    private int CalculateAmount()
    {
        int value = Random.Range(0, 100);
        int final;

        if (value <= 60) final = 5;
        else if (value > 60 && value < 90) final = 10;
        else if (value >= 90 && value < 97) final = 15;
        else final = 20;

        return final;
    }
    private int CalculateNewValue(TypeFlair flair, int amount, bool increment)
    {
        int value;

        if (increment)
        {
            if(flair == TypeFlair.MissChance) value = (int)_player.GetterStats((int)flair, true) - amount;
            else value = (int)_player.GetterStats((int)flair, true) + amount;
        }
        else
        {
            if (flair == TypeFlair.MissChance) value = (int)_player.GetterStats((int)flair, true) + amount;
            else value = (int)_player.GetterStats((int)flair, true) - amount;
        }

        return value;
    }
}
