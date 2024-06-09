using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [SerializeField] private float[] _generalMaxStats;
    private float[] _generalStats;

    [Header("More Stats")]
    private int countKillsInRoom;
    private int countKillsTotal;
    private int countDamageInRoom;
    private int countDamageTotal;
    private int countDamageReceivedInRoom;
    private int countDamageReceivedTotal;

    [Header("Content UI")]
    public GameObject statsWindow;
    public TextMeshProUGUI[] textStats;
    public Image[] _imgObject;

    [Header("Content Skills UI")]
    public Image[] _imgSkills;
    public TextMeshProUGUI[] _nameSkills;
    public TextMeshProUGUI[] _descSkills;

    [Header("Player Content")]
    public List<SkillManager> skills = new List<SkillManager>();
    public List<Object> objects = new List<Object>();
    private TriggeringObject triggering;

    // EVENTOS
    public static event Action takeDamage;

    private void Awake()
    {
        triggering = GetComponent<TriggeringObject>();
    }
    private void Start()
    {
        #region InitialStats
        _generalStats = new float[_generalMaxStats.Length];

        for(int i = 0; i < _generalMaxStats.Length; i++)
        {
            _generalStats[i] = _generalMaxStats[i];
        }
        #endregion
        // SETTEAR VALORES INICIALES PARA LA UI -------- //
        SetChangeSkillsInUI();
        ChangeValueInUI(-1);
        // --------------------------------------------- //
        #region OffElements
        for (int i = 0; i < _imgSkills.Length; i++)
        {
            _imgSkills[i].gameObject.SetActive(false);
            _nameSkills[i].gameObject.SetActive(false);
            _descSkills[i].gameObject.SetActive(false);
        }
        statsWindow.SetActive(false);
        #endregion

        ActionForControlPlayer.skillOne += () => LaunchedSkill(0);
        ActionForControlPlayer.skillTwo += () => LaunchedSkill(1);
        ActionForControlPlayer.skillFragments += () => LaunchedSkill(2);
    }
    public void ShowWindowedStats()
    {
        if (Pause.state == State.Interface)
        {
            Pause.StateChange = State.Game;
            statsWindow.SetActive(false);
        }
        else
        {
            Pause.StateChange = State.Interface;
            statsWindow.SetActive(true);
        }
    }
    // ---- SETTERS ---- //
    public void SetValue(int type, float value, bool max = true)
    {
        if(value < 0) FloatTextManager.CreateText(transform.position, (TypeColor)type, ("-" + value.ToString()));
        else FloatTextManager.CreateText(transform.position, (TypeColor)type, ("+" + value.ToString()));

        if (max) _generalMaxStats[type] += value;
        else _generalStats[type] += value;

        ChangeValueInUI(type);
    }
    public void TakeDamage(int dmg)
    {
        takeDamage?.Invoke();

        FloatTextManager.CreateText(transform.position, TypeColor.receivedDamage, ("-" + dmg.ToString()));

        SetValue(0, dmg, false);
    }
    public void SetChangeSkillsInUI()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            _imgSkills[i].gameObject.SetActive(true);
            _nameSkills[i].gameObject.SetActive(true);
            _descSkills[i].gameObject.SetActive(true);

            _imgSkills[i].sprite = skills[i].icon;
            _nameSkills[i].text = LanguageManager.GetValue("Skill", skills[i].skillName);
            _descSkills[i].text = LanguageManager.GetValue("Skill", skills[i].descName);
        }
    }
    // ---- GETTERS ---- //
    public float GetterStats(int pos, bool max = true)
    {
        if(max) return _generalMaxStats[pos];
        else return _generalStats[pos];
    }
    // ---- OBJECTS ---- //
    public void AddObject(Object obj)
    {
        objects.Add(obj);
        triggering.SetObjects(objects);

        _imgObject[objects.Count - 1].sprite = objects[objects.Count - 1].icon;
        _imgObject[objects.Count - 1].color = Color.white;
    }
    // ---- SKILLS ---- //
    private void LaunchedSkill(int pos)
    {
        if (skills.Count <= pos) return;

        if (skills[pos].loadType == LoadTypeSkill.concentration)
        {
            if (_generalStats[1] >= skills[pos].amountFuel) CreateSkill(pos);
        }

        if (skills[pos].loadType == LoadTypeSkill.kills)
        {
            if (countKillsInRoom >= skills[pos].amountFuel) CreateSkill(pos);
        }

        if (skills[pos].loadType == LoadTypeSkill.receiveDamage)
        {
            if (countDamageReceivedInRoom >= skills[pos].amountFuel) CreateSkill(pos);
        }

        if (skills[pos].loadType == LoadTypeSkill.damage)
        {
            if (countDamageInRoom >= skills[pos].amountFuel) CreateSkill(pos);
        }
    }
    private void CreateSkill(int id)
    {
        Vector3 position = this.transform.position;
        if (skills[id].typeShow == TypeShowSkill.created)
        {
            // CREAR ENCIMA DEL ENEMIGO AL QUE LE APUNTA EL JUGADOR
        }

        LessAmountPerSkill(id);

        for(int i = 0; i < skills[id].countCreated; i++) { Instantiate(skills[id].gameObject, position, Quaternion.identity); }
    }
    private void LessAmountPerSkill(int pos)
    {
        if (skills[pos].loadType == LoadTypeSkill.concentration) SetValue(1, -skills[pos].amountFuel, false);

        if (skills[pos].loadType == LoadTypeSkill.damage) countDamageInRoom -= skills[pos].amountFuel;

        if (skills[pos].loadType == LoadTypeSkill.receiveDamage) countDamageReceivedInRoom -= skills[pos].amountFuel;
    }
    // ---- FUNCION INTEGRA ---- //
    private void ChangeValueInUI(int type)
    {
        if (type == -1) { for (int i = 0; i < _generalStats.Length; i++) { ChangeStatsInUI(i); } }
        else { ChangeStatsInUI(type); }
    }
    protected void ChangeStatsInUI(int i)
    {
        if (i == 0 || i == 1 || i == 10)
            textStats[i].text = _generalStats[i].ToString() + "/" + _generalMaxStats[i].ToString();
        else if (i == 2 || i == 3 || i == 6 || i == 7 || i == 8 || i == 9)
            textStats[i].text = _generalMaxStats[i].ToString() + "%";
        else if (i == 4 || i == 5)
            textStats[i].text = _generalMaxStats[i].ToString();
    }
}
