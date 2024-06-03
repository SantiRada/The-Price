using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [SerializeField] private int _hpMax;
    [SerializeField] private int _concentrationMax;
    [SerializeField] private int _speedMoveMax;
    [SerializeField] private float _speedAttackMax;
    [SerializeField] private int _skillDamageMax;
    [SerializeField] private int _damageMax;
    private int _hp;
    private int _concentration;
    private int _speedMove;
    private float _speedAttack;
    private int _skillDamage;
    private int _damage;

    [Header("Attack Values")]
    [SerializeField] private int _subsequentDamageMax;
    [SerializeField] private int _criticChanceMax;
    [SerializeField] private int _missChanceMax;
    private int _subsequentDamage;
    private int _criticChance;
    private int _missChance;

    [Header("Modifiers")]
    [SerializeField] private int _hpStealingMax;
    [SerializeField] private int _sanityMax;
    private int _hpStealing;
    private int _sanity;

    [Header("Content UI")]
    public GameObject statsWindow;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI concentrationText;
    public TextMeshProUGUI speedMoveText;
    public TextMeshProUGUI speedAttackText;
    public TextMeshProUGUI skillDamageText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI subsequentDamageText;
    public TextMeshProUGUI criticChanceText;
    public TextMeshProUGUI missChanceText;
    public TextMeshProUGUI hpStealingText;
    public TextMeshProUGUI sanityText;
    [Space]
    public Image[] _imgSkills;
    public TextMeshProUGUI[] _nameSkills;
    public TextMeshProUGUI[] _descSkills;

    [Header("Player Content")]
    public List<SkillManager> skills = new List<SkillManager>();
    public List<Object> objects = new List<Object>();
    [HideInInspector] public event Action takeDamage;
    private TriggeringObject _triggering;

    private void Awake()
    {
        _triggering = GetComponent<TriggeringObject>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        for (int i = 0; i < _imgSkills.Length; i++)
        {
            _imgSkills[i].gameObject.SetActive(false);
            _nameSkills[i].gameObject.SetActive(false);
            _descSkills[i].gameObject.SetActive(false);
        }

        #region Values Base for Stats
        _hp = _hpMax;
        _concentration = _concentrationMax;
        _speedMove = _speedMoveMax;
        _speedAttack = _speedAttackMax;
        _skillDamage = _skillDamageMax;
        _damage = _damageMax;
        _subsequentDamage = _subsequentDamageMax;
        _criticChance = _criticChanceMax;
        _missChance = _missChanceMax;
        _hpStealing = _hpStealingMax;
        _sanity = _sanityMax;
        #endregion

        SetChangeSkillsInUI();
        ChangeValueInUI(-1);

        statsWindow.SetActive(false);
    }
    private void ChangeValueInUI(int type)
    {
        if (type == 0 || type == -1) hpText.text = _hp.ToString() + "/" + _hpMax.ToString();
        if (type == 1 || type == -1) concentrationText.text = _concentration.ToString() + "/" + _concentrationMax.ToString();
        if (type == 2 || type == -1) speedMoveText.text = _speedMove.ToString();
        if (type == 3 || type == -1) speedAttackText.text = _speedAttack.ToString();
        if (type == 4 || type == -1) skillDamageText.text = _skillDamage.ToString();
        if (type == 5 || type == -1) damageText.text = _damage.ToString();
        if (type == 6 || type == -1) subsequentDamageText.text = _subsequentDamage.ToString() + "%";
        if (type == 7 || type == -1) criticChanceText.text = _criticChance.ToString() + "%";
        if (type == 8 || type == -1) missChanceText.text = _missChance.ToString() + "%";
        if (type == 9 || type == -1) hpStealingText.text = _hpStealing.ToString() + "%";
        if (type == 10 || type == -1) sanityText.text = _sanity.ToString() + "/" + _sanityMax.ToString();
    }
    public void SetValue(int type, float value, int modifier)
    {
        if(modifier == -1)
        {
            if (type == 0) _hp -= (int)value;
            if (type == 1) _concentration -= (int)value;
            if (type == 2) _speedMove -= (int)value;
            if (type == 3) _speedAttack -= value;
            if (type == 4) _skillDamage -= (int)value;
            if (type == 5) _damage -= (int)value;
            if (type == 6) _subsequentDamage -= (int)value;
            if (type == 7) _criticChance -= (int)value;
            if (type == 8) _missChance -= (int)value;
            if (type == 9) _hpStealing -= (int)value;
            if (type == 10) _sanity -= (int)value;
        }
        else if(modifier == 0)
        {
            if (type == 0) _hp = (int)value;
            if (type == 1) _concentration = (int)value;
            if (type == 2) _speedMove = (int)value;
            if (type == 3) _speedAttack = value;
            if (type == 4) _skillDamage = (int)value;
            if (type == 5) _damage = (int)value;
            if (type == 6) _subsequentDamage = (int)value;
            if (type == 7) _criticChance = (int)value;
            if (type == 8) _missChance = (int)value;
            if (type == 9) _hpStealing = (int)value;
            if (type == 10) _sanity = (int)value;
        }
        else
        {
            if (type == 0) _hp += (int)value;
            if (type == 1) _concentration += (int)value;
            if (type == 2) _speedMove += (int)value;
            if (type == 3) _speedAttack += value;
            if (type == 4) _skillDamage += (int)value;
            if (type == 5) _damage += (int)value;
            if (type == 6) _subsequentDamage += (int)value;
            if (type == 7) _criticChance += (int)value;
            if (type == 8) _missChance += (int)value;
            if (type == 9) _hpStealing += (int)value;
            if (type == 10) _sanity += (int)value;
        }

        ChangeValueInUI(type);
    }
    public void ShowStats()
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
    public void TakeDamage(int dmg)
    {
        takeDamage?.Invoke();

        _hp -= dmg;
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
    public void SetterStats(int pos, float value)
    {
        switch (pos)
        {
            case 0: _hp = (int)value; break;
            case 1: _concentration = (int)value; break;
            case 2: _speedMove = (int)value; break;
            case 3: _speedAttack = value; break;
            case 4: _skillDamage = (int)value; break;
            case 5: _damage = (int)value; break;
            case 6: _subsequentDamage = (int)value; break;
            case 7: _criticChance = (int)value; break;
            case 8: _missChance = (int)value; break;
            case 9: _hpStealing = (int)value; break;
            case 10: _sanity = (int)value; break;
        }

        ChangeValueInUI(pos);
    }
    public void SetterMaxStats(int pos, float value)
    {
        switch (pos)
        {
            case 0: _hpMax = (int)value; break;
            case 1: _concentrationMax = (int)value; break;
            case 2: _speedMoveMax = (int)value; break;
            case 3: _speedAttackMax = value; break;
            case 4: _skillDamageMax = (int)value; break;
            case 5: _damageMax = (int)value; break;
            case 6: _subsequentDamageMax = (int)value; break;
            case 7: _criticChanceMax = (int)value; break;
            case 8: _missChanceMax = (int)value; break;
            case 9: _hpStealingMax = (int)value; break;
            case 10: _sanityMax = (int)value; break;
        }

        ChangeValueInUI(pos);
    }
    // ---- GETTERS ---- //
    public float GetterStats(int pos)
    {
        float value = 0;
        switch (pos)
        {
            case 0: value = _hp; break;
            case 1: value = _concentration; break;
            case 2: value = _speedMove; break;
            case 3: value = _speedAttack; break;
            case 4: value = _skillDamage; break;
            case 5: value = _damage; break;
            case 6: value = _subsequentDamage; break;
            case 7: value = _criticChance; break;
            case 8: value = _missChance; break;
            case 9: value = _hpStealing; break;
            case 10: value = _sanity; break;
        }

        return value;
    }
    public float GetterMaxStats(int pos)
    {
        float value = 0;
        switch (pos)
        {
            case 0: value = _hpMax; break;
            case 1: value = _concentrationMax; break;
            case 2: value = _speedMoveMax; break;
            case 3: value = _speedAttackMax; break;
            case 4: value = _skillDamageMax; break;
            case 5: value = _damageMax; break;
            case 6: value = _subsequentDamageMax; break;
            case 7: value = _criticChanceMax; break;
            case 8: value = _missChanceMax; break;
            case 9: value = _hpStealingMax; break;
            case 10: value = _sanityMax; break;
        }

        return value;
    }
    // ---- OBJECTS ---- //
    public void AddObject(Object obj)
    {
        objects.Add(obj);
        _triggering.GetObjects();
    }
}
