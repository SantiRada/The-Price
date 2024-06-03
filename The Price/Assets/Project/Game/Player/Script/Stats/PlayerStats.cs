using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [SerializeField] private int _hp;
    [SerializeField] private int _concentration;
    [SerializeField] private int _speedMove;
    [SerializeField] private float _speedAttack;
    [SerializeField] private int _skillDamage;
    [SerializeField] private int _damage;

    [Header("Attack Values")]
    [SerializeField] private int _subsequentDamage;
    [SerializeField] private int _criticChance;
    [SerializeField] private int _missChance;

    [Header("Modifiers")]
    [SerializeField] private int _hpStealing;
    [SerializeField] private int _sanity;

    [Header("Private Values")]
    private int _hpMax;
    private int _concentrationMax;
    private int _speedMoveMax;
    private float _speedAttackMax;
    private int _skillDamageMax;
    private int _damageMax;
    private int _subsequentDamageMax;
    private int _criticChanceMax;
    private int _missChanceMax;
    private int _hpStealingMax;
    private int _sanityMax;

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

    [Header("Private Content")]
    private PlayerMovement _movement;
    private List<SkillManager> _skills = new List<SkillManager>();
    [HideInInspector] public event Action takeDamage;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
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
        _hpMax = _hp;
        _concentrationMax = _concentration;
        _speedMoveMax = _speedMove;
        _speedAttackMax = _speedAttack;
        _skillDamageMax = _skillDamage;
        _damageMax = _damage;
        _subsequentDamageMax = _subsequentDamage;
        _criticChanceMax = _criticChance;
        _missChanceMax = _missChance;
        _hpStealingMax = _hpStealing;
        _sanityMax = _sanity;
        #endregion

        SetChangeSkills();
        ChangeValueInUI(-1);

        statsWindow.SetActive(false);
    }
    private void ChangeValueInUI(int type)
    {
        if (type == 0 || type == -1) hpText.text = _hp.ToString();
        if (type == 1 || type == -1) concentrationText.text = _concentration.ToString();
        if (type == 2 || type == -1) speedMoveText.text = _speedMove.ToString();
        if (type == 3 || type == -1) speedAttackText.text = _speedAttack.ToString();
        if (type == 4 || type == -1) skillDamageText.text = _damage.ToString();
        if (type == 5 || type == -1) damageText.text = _damage.ToString();
        if (type == 6 || type == -1) subsequentDamageText.text = _subsequentDamage.ToString() + "%";
        if (type == 7 || type == -1) criticChanceText.text = _criticChance.ToString() + "%";
        if (type == 8 || type == -1) missChanceText.text = _missChance.ToString() + "%";
        if (type == 9 || type == -1) hpStealingText.text = _hpStealing.ToString() + "%";
        if (type == 10 || type == -1) sanityText.text = _sanity.ToString();
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
    public void SetChangeSkills()
    {
        _skills = _movement.skills;

        for(int i = 0; i < _skills.Count; i++)
        {
            _imgSkills[i].gameObject.SetActive(true);
            _nameSkills[i].gameObject.SetActive(true);
            _descSkills[i].gameObject.SetActive(true);

            _imgSkills[i].sprite = _skills[i].icon;
            _nameSkills[i].text = LanguageManager.GetValue("Skill", _skills[i].skillName);
            _descSkills[i].text = LanguageManager.GetValue("Skill", _skills[i].descName);
        }
    }
    public void ShowStats()
    {
        if(Pause.state == State.Interface)
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
    public void TakeDamage(int value)
    {
        _hp -= value;
        ChangeValueInUI(0);

        takeDamage?.Invoke();
    }
    public int HarvestConcentration { set { _concentration += value; ChangeValueInUI(1); } }
    // ---- GETTERS ---- //
    public int HP { get { return _hp; } }
    public int Concentration { get { return _concentration; } }
    public int SpeedMove { get {  return _speedMove; } }
    public float SpeedAttack { get {  return _speedAttack; } }
    public int SkillDamage { get { return _skillDamage; } }
    public int Damage { get { return _damage; } }
    public int SubsequentDamage { get { return _subsequentDamage; } }
    public int CriticChance { get {  return _criticChance; } }
    public int MissChance {  get { return _missChance; } }
    public int StealingHP { get { return _hpStealing; } }
    public int Sanity { get { return _sanity; } }
}
