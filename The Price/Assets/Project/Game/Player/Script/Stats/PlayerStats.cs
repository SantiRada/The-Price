using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [Tooltip("Tiempo que dura el color Rojo del 'Take Damage'")] public float timeToTakeDamage;
    [SerializeField] private float[] _generalMaxStats;
    private float[] _generalStats;
    private bool _canReceivedDamage { get; set; }

    [Header("More Stats")]
    [HideInInspector] public int countKillsInRoom;
    [HideInInspector] public int countDamageInRoom;
    [HideInInspector] public int countDamageReceivedInRoom;

    [Header("Content UI")]
    public GameObject dieUI;
    public GameObject statsWindow;
    public TextMeshProUGUI[] textStats;
    public Image[] _imgObject;
    private HUD _hud;

    [Header("Content Skills UI")]
    public Image[] _imgSkills;
    public TextMeshProUGUI[] _nameSkills;
    public TextMeshProUGUI[] _descSkills;

    [Header("Weapons")]
    public WeaponSystem weapon;
    public GameObject weaponParent;

    [Header("Player Content")]
    [HideInInspector] public List<SkillManager> skills = new List<SkillManager>();
    [HideInInspector] public List<Object> objects = new List<Object>();
    private TriggeringObject triggering;
    private PlayerMovement _movement;
    private SpriteRenderer _spr;

    [Header("Prevent Damage Per Type")]
    [HideInInspector] public int[] countPrevent = new int[5];
    [HideInInspector] public bool[] whichReflect = new bool[5];
    [Space]
    [HideInInspector] public int[] preventDistance = new int[2];
    [HideInInspector] public bool[] reflectDistance = new bool[2];

    [Header("Data States")]
    [HideInInspector] public TypeState state;
    [HideInInspector] public int numberOfLoads;

    // EVENTOS
    public static event Action takeDamage;
    public static event Action jumpBetween;

    private void Awake()
    {
        triggering = GetComponent<TriggeringObject>();
        _movement = GetComponent<PlayerMovement>();
        _spr = GetComponent<SpriteRenderer>();
        _hud = FindAnyObjectByType<HUD>();
    }
    private void Start()
    {
        dieUI.gameObject.SetActive(false);

        #region InitialStats
        CanReceivedDamage = true;

        _generalStats = new float[_generalMaxStats.Length];

        for(int i = 0; i < _generalMaxStats.Length; i++)
        {
            _generalStats[i] = _generalMaxStats[i];
        }

        if (weapon != null) InitialWeapon();
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
    // -------------- DEAD -------------- //
    private IEnumerator Die()
    {
        CameraMovement.Shake(0.05f, 0.45f);
        yield return new WaitForSeconds(0.15f);

        CameraMovement.SetDie();
        Pause.StateChange = State.Pause;
        yield return new WaitForSeconds(0.5f);

        dieUI.gameObject.SetActive(true);
    }
    // ---- FUNCIONES BASE ---- //
    public void TakeDamage(GameObject obj, int dmg)
    {
        if (!_canReceivedDamage) return;

        #region ComprobateEnemy
        EnemyManager attacker;

        if (obj.GetComponent<EnemyManager>()) { attacker = obj.GetComponent<EnemyManager>(); }
        else { return; }

        // ---- PREVIENE ATAQUES DE UN TIPO ESPECÍFICO ---- //
        dmg = (attacker != null) ? CalculateNewDamage(attacker, dmg) : dmg;
        if (dmg <= 0) return;
        #endregion

        // EVENTO = RECIBIR DAÑO
        takeDamage?.Invoke();

        #region ApplyDamage
        SetValue(0, -dmg, false);
        _hud.SetHealthbar(_generalStats[0], _generalMaxStats[0]);
        _spr.color = Color.red;
        Invoke("ResetColor", timeToTakeDamage);

        _movement.SetValuesForcedMove(obj);

        CanReceivedDamage = false;
        #endregion

        // VERIFICAR SI SIGUE VIVO
        if (_generalStats[0] <= 0) StartCoroutine("Die");
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
    // ---- FUNCIONES DE OTROS SCRIPT ---- //
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
    public void JumpBetweenAttack() { jumpBetween?.Invoke(); }
    // ---- SETTERS ---- //
    public void PreventDamagePerType(int[] count, bool[] reflects)
    {
        countPrevent = count;
        whichReflect = reflects;
    }
    public void PreventDamagePerDistance(int[] count, bool[] reflects)
    {
        preventDistance = count;
        reflectDistance = reflects;
    }
    public void AddStatePerDamage(TypeState st, int number)
    {
        state = st;
        numberOfLoads = number;
    }
    public void SetValue(int type, float value, bool max = true, bool canShow = true)
    {
        if (value == 0) return;

        if (canShow)
        {
            if (value < 0) FloatTextManager.CreateText(transform.position, (TypeColor)type, value.ToString());
            else FloatTextManager.CreateText(transform.position, (TypeColor)type, ("+" + value.ToString()));
        }

        if (max) _generalMaxStats[type] += value;
        else _generalStats[type] += value;

        // CHANGE IN HUD
        if (type == 1 && !max) _hud.SetConcentracion(_generalStats[1], _generalMaxStats[1]);

        ChangeValueInUI(type);
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

            _hud.SetSkills(i, skills[i].icon);
        }
    }
    // ---- GETTERS ---- //
    public float GetterStats(int pos, bool max = true)
    {
        if (max) return _generalMaxStats[pos];
        else return _generalStats[pos];
    }
    public bool CanReceivedDamage { get { return _canReceivedDamage; } set { _canReceivedDamage = value; } }
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
    private int CalculateNewDamage(EnemyManager attacker, int dmg)
    {
        #region Verify Prevent & Reflect Damage Per Type
        bool dmgPrevent = false;
        if (attacker.typeAttack == TypeEnemyAttack.Energy)
        {
            if (whichReflect[1]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (countPrevent[1] / 100));
            dmgPrevent = true;
        }
        if (attacker.typeAttack == TypeEnemyAttack.Fire)
        {
            if (whichReflect[2]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (countPrevent[2] / 100));
            dmgPrevent = true;
        }
        if (attacker.typeAttack == TypeEnemyAttack.Cold)
        {
            if (whichReflect[3]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (countPrevent[3] / 100));
            dmgPrevent = true;
        }
        if (attacker.typeAttack == TypeEnemyAttack.Fortify)
        {
            if (whichReflect[4]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (countPrevent[4] / 100));
            dmgPrevent = true;
        }
        if (!dmgPrevent)
        {
            if (whichReflect[0]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (countPrevent[0] / 100));
        }
        #endregion

        #region Verify Prevent & Reflect Damage Per Distance
        // VERIFICACIÓN POR MELEE
        if (!attacker.distanceAttack && preventDistance[0] != 0)
        {
            if (reflectDistance[0]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (preventDistance[0] / 100));
        }
        // VERIFICACIÓN POR DISTANCIA
        if(attacker.distanceAttack && preventDistance[1] != 0)
        {
            if (reflectDistance[1]) attacker.TakeDamage(dmg);
            dmg -= (dmg * (preventDistance[1] / 100));
        }
        #endregion

        return dmg;
    }
    private void InitialWeapon()
    {
        Instantiate(weapon.gameObject, weaponParent.transform.position, Quaternion.identity, weaponParent.transform);
    }
    private void ResetColor()
    {
        _spr.color = Color.white;
        CanReceivedDamage = true;
    }
}