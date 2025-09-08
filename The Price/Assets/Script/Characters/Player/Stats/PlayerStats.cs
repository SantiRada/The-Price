using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [Tooltip("Tiempo que dura el color Rojo del 'Take Damage'")] public float timeToTakeDamage;
    [Tooltip("Tiempo de intangibilidad tras recibir daño")] public float timeToIntangible;
    [SerializeField] private float[] _generalMaxStats;
    private float[] _generalStats;
    public bool _canReceivedDamage;

    [Header("Changers")]
    public float changerConcentration;

    [Header("More Stats")]
    [HideInInspector] public int countKillsInRoom;
    [HideInInspector] public int countDamageInRoom;
    [HideInInspector] public int countDamageReceivedInRoom;

    [Header("Weapons")]
    public List<WeaponSystem> weapons = new List<WeaponSystem>();
    public GameObject weaponParent;
    [HideInInspector] public List<WeaponSystem> weaponInScene = new List<WeaponSystem>();
    [Space]
    public float delayToAttack;
    public bool canLaunchAttack;

    [Header("Player Content")]
    public List<SkillManager> skills = new List<SkillManager>();
    public List<Object> objects = new List<Object>();
    
    [HideInInspector] public DeadSystem deadSystem;
    private TriggeringObject triggering;
    private StatsInUI _statsInUI;
    private SpriteRenderer _spr;

    [HideInInspector] public bool isDead = false;

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
    public static event Action criticalChance;
    public static event Action missChance;
    // EVENTO INTEGRO
    public static event Action changesInWeapons;

    private void Awake()
    {
        triggering = GetComponent<TriggeringObject>();
        _statsInUI = FindAnyObjectByType<StatsInUI>();
        deadSystem = GetComponent<DeadSystem>();
        _spr = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        _generalStats = new float[_generalMaxStats.Length];
        for (int i = 0; i < _generalMaxStats.Length; i++) { _generalStats[i] = _generalMaxStats[i]; }

        canLaunchAttack = true;

        if (weapons != null) InitialWeapon();
    }
    private void Start()
    {
        _canReceivedDamage = true;

        ActionForControlPlayer.skillOne += () => LaunchedSkill(0);
        ActionForControlPlayer.skillTwo += () => LaunchedSkill(1);
        ActionForControlPlayer.skillFragments += () => LaunchedSkill(2);
    }
    // -------------- DEAD -------------- //
    private IEnumerator Die()
    {
        CameraMovement.Shake(0.05f, 0.45f);
        yield return new WaitForSeconds(0.15f);

        isDead = true;
        CameraMovement.SetDie();
        Pause.StateChange = State.Interface;
        yield return new WaitForSeconds(0.5f);

        _statsInUI.dieUI.gameObject.SetActive(true);
    }
    // ---- FUNCIONES BASE ---- //
    public void TakeDamage(GameObject obj, int dmg)
    {
        if (!_canReceivedDamage) { return; }

        #region ComprobateEnemy
        EnemyManager attacker;

        if (obj.GetComponent<EnemyManager>())
        {
            attacker = obj.GetComponent<EnemyManager>();

            // ---- PREVIENE ATAQUES DE UN TIPO ESPECÍFICO ---- //
            dmg = (attacker != null) ? CalculateNewDamage(attacker, dmg) : dmg;
        }

        if (dmg <= 0)
        {
            Debug.Log("No hay daño suficiente");
            return;
        }
        #endregion

        // EVENTO = RECIBIR DAÑO
        takeDamage?.Invoke();

        #region ApplyDamage
        SetValue(0, -dmg, false);

        if (_statsInUI == null) return;
        _statsInUI.SetHUD(0, _generalStats[0], _generalMaxStats[0]);

        // GUARDAR DAÑO RECIBIDO
        countDamageReceivedInRoom += dmg;


        // APLICAR ESTADO DE INTANGIBILIDAD
        _canReceivedDamage = false;
        // CAMBIAR COLOR POR UN PEQUEÑO PERIODO DE TIEMPO
        _spr.color = Color.red;
        Invoke("ResetColor", timeToTakeDamage);
        // QUITAR ESTADO DE INTANGIBILIDAD
        Invoke("RemoveStateIntangible", timeToIntangible);
        #endregion

        // VERIFICAR SI SIGUE VIVO
        if (_generalStats[0] <= 0) StartCoroutine("Die");
    }
    public void ApplyDamage(int value)
    {
        int percentage = (int)_generalStats[9] * value / 100;

        SetValue(0, percentage, false, true);
        _statsInUI.SetHUD(0, _generalStats[0], _generalMaxStats[0]);
    }
    // ---- OBJECTS ---- //
    public void AddObject(Object obj)
    {
        objects.Add(obj);
        triggering.SetObjects(objects);

        _statsInUI.AddObjectInUI();
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
    public void SetValue(int type, float value, bool max = true, bool canShow = true, bool equalValue = false)
    {
        if (value == 0) return;

        if (_generalStats[type] == _generalMaxStats[type] && !max && value > 0) return;

        if (canShow)
        {
            if (value < 0) FloatTextManager.CreateText(transform.position, (TypeColor)type, value.ToString());
            else FloatTextManager.CreateText(transform.position, (TypeColor)type, ("+" + value.ToString()));
        }

        if (equalValue)
        {
            if (max) _generalMaxStats[type] = value;
            else _generalStats[type] = value;
        }
        else
        {
            if (max) { _generalMaxStats[type] += value; }
            else
            {
                // COMPROBACION NECESARIA PARA NO PASARSE DEL VALOR MÁXIMO DE STATS
                if ((_generalStats[type] + value) > _generalMaxStats[type]) { _generalStats[type] = value; }
                else { _generalStats[type] += value; }
            }
        }

        // CHANGE IN HUD
        if (type == 1 && !max) _statsInUI.SetHUD(1, _generalStats[1], _generalMaxStats[1]);

        if(_statsInUI != null )_statsInUI.ChangeValueInUI(type);
    }
    public float ChangerConcentration { set { changerConcentration = value; } get { return changerConcentration; } }
    public int SetWeapon(int index, WeaponSystem weapon)
    {
        _statsInUI.SetWeaponInHUD(index, weapon.spr);

        if(weapons.Count > index)
        {
            if (weapons[index] == null)
            {
                weapons[index] = weapon;
                CreateWeaponInScene(true, index);
                return -1;
            }
            else
            {
                GameObject prevObject = weaponInScene[index].gameObject;
                int prevWeapon = weaponInScene[index].weaponID;

                weapons[index] = weapon;
                CreateWeaponInScene(false, index);

                Destroy(prevObject, 1f);

                return prevWeapon;
            }
        }
        else
        {
            for(int i = 0; i < index; i++)
            {
                if(weapons.Count <= i) { weapons.Add(null); }
            }

            weapons.Add(weapon);
            CreateWeaponInScene(true, index);

            return -1;
        }
    }
    public void UpdateWeaponInAction() { changesInWeapons?.Invoke(); }
    // ----  ---- //
    public void SetCountKills() { countKillsInRoom++; }
    public void SetCountDamage(int count) { countDamageInRoom += count; }
    public void ResetValuesPerRoom()
    {
        countKillsInRoom = 0;
        countDamageInRoom = 0;
        countDamageReceivedInRoom = 0;
    }
    public void LaunchAttack() { StartCoroutine(LaunchAttackDelay()); }
    private IEnumerator LaunchAttackDelay()
    {
        canLaunchAttack = false;

        yield return new WaitForSeconds(delayToAttack);

        canLaunchAttack = true;
    }
    // ---- GETTERS ---- //
    public float GetterStats(int pos, bool max = true)
    {
        if (max) return _generalMaxStats[pos];
        else return _generalStats[pos];
    }
    // ---- FUNCION INTEGRA ---- //
    private int CalculateNewDamage(EnemyManager attacker, int dmg)
    {
        if (attacker == null) return 0;

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
        if(weapons.Count > 0)
        {
            for(int i = 0; i < weapons.Count; i++)
            {
                if(weapons[i] != null) weaponInScene.Add(Instantiate(weapons[i].gameObject, weaponParent.transform.position, Quaternion.identity, weaponParent.transform).GetComponent<WeaponSystem>());
            }
        }
        changesInWeapons?.Invoke();
    }
    private void CreateWeaponInScene(bool isNew, int index = 0)
    {
        if (!isNew)
        {
            weaponInScene[index] = Instantiate(weapons[index].gameObject, weaponParent.transform.position, Quaternion.identity, weaponParent.transform).GetComponent<WeaponSystem>();
        }
        else
        {
            for (int i = 0; i < index; i++)
            {
                if (weaponInScene.Count <= i) { weaponInScene.Add(null); }
            }

            if(weaponInScene.Count > index)
            {
                weaponInScene[index] = Instantiate(weapons[index].gameObject, weaponParent.transform.position, Quaternion.identity, weaponParent.transform).GetComponent<WeaponSystem>();
            }
            else
            {
                weaponInScene.Add(Instantiate(weapons[index].gameObject, weaponParent.transform.position, Quaternion.identity, weaponParent.transform).GetComponent<WeaponSystem>());
            }
        }

        changesInWeapons?.Invoke();
    }
    private void ResetColor() { _spr.color = Color.white; }
    private void RemoveStateIntangible() { _canReceivedDamage = true; }
    // ---- FUNCION INTEGRA: WEAPON ---- //
    public bool ComprobationMissChance()
    {
        int rnd = UnityEngine.Random.Range(0, 100);

        if(rnd < _generalStats[8])
        {
            missChance?.Invoke();

            FloatTextManager.CreateText(transform.position, TypeColor.MissChance, "47", false, true);

            return true;
        }
        else { return false; }
    }
    public bool ComprobationCriticalChance()
    {
        int rnd = UnityEngine.Random.Range(0, 100);

        if (rnd < _generalStats[7])
        {
            criticalChance?.Invoke();

            FloatTextManager.CreateText(transform.position, TypeColor.CriticalChance, "48", false, true);

            return true;
        }
        else { return false; }
    }
}