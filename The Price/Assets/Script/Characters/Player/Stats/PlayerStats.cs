using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maneja todas las estadísticas del jugador, su sistema de daño, armas, habilidades y objetos.
/// Simplificado: 7 stats, 1 slot de arma, 2 slots de habilidad.
/// </summary>
public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [Tooltip("Tiempo que dura el color Rojo del 'Take Damage'")] public float timeToTakeDamage;
    [Tooltip("Tiempo de intangibilidad tras recibir daño")] public float timeToIntangible;
    [SerializeField] private float[] _generalMaxStats; // 7 stats: PV, Concentración, VelMov, VelAtk, SkillDmg, Dmg, CritChance
    private float[] _generalStats;
    public bool _canReceivedDamage;

    [Header("Changers")]
    public float changerConcentration;

    [Header("More Stats")]
    [HideInInspector] public int countKillsInRoom;
    [HideInInspector] public int countDamageInRoom;
    [HideInInspector] public int countDamageReceivedInRoom;

    [Header("Weapon - Solo 1 slot")]
    public WeaponSystem weapon; // Solo un arma
    public GameObject weaponParent;
    [HideInInspector] public WeaponSystem weaponInScene;
    [Space]
    public float delayToAttack;
    public bool canLaunchAttack;

    [Header("Player Content - 2 slots de habilidad")]
    public List<SkillManager> skills = new List<SkillManager>(); // Máximo 2
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
    public static event Action criticalChance;
    public static event Action weaponChanged;

    private void Awake()
    {
        InitializeComponents();
        ValidateConfiguration();
    }

    /// <summary>
    /// Inicializa y valida todos los componentes necesarios
    /// </summary>
    private void InitializeComponents()
    {
        // Componentes locales - críticos
        if (!this.TryGetComponentSafe(out triggering))
        {
            Debug.LogError($"[PlayerStats] TriggeringObject no encontrado en {gameObject.name}. El sistema de objetos no funcionará correctamente.");
        }

        if (!this.TryGetComponentSafe(out deadSystem))
        {
            Debug.LogError($"[PlayerStats] DeadSystem no encontrado en {gameObject.name}. El sistema de muerte no funcionará.");
        }

        if (!this.TryGetComponentSafe(out _spr))
        {
            Debug.LogError($"[PlayerStats] SpriteRenderer no encontrado en {gameObject.name}. Los efectos visuales de daño no funcionarán.");
        }

        // Componentes de escena - pueden no existir en escenas de prueba
        if (!ComponentHelper.TryFindObjectSafe(out _statsInUI, nameof(PlayerStats)))
        {
            Debug.LogWarning($"[PlayerStats] StatsInUI no encontrado. La interfaz de usuario no se actualizará.");
        }
    }

    /// <summary>
    /// Valida la configuración del jugador
    /// </summary>
    private void ValidateConfiguration()
    {
        if (_generalMaxStats == null || _generalMaxStats.Length != 7)
        {
            Debug.LogError($"[PlayerStats] _generalMaxStats debe tener exactamente 7 stats. Usando valores por defecto.");
            _generalMaxStats = new float[7]; // 7 stats
        }

        if (weaponParent == null)
        {
            Debug.LogWarning($"[PlayerStats] weaponParent no está asignado. Las armas no se crearán correctamente.");
        }

        // Validar que solo haya máximo 2 habilidades
        if (skills.Count > 2)
        {
            Debug.LogWarning($"[PlayerStats] Se encontraron {skills.Count} habilidades, pero solo se permiten 2. Eliminando extras.");
            skills.RemoveRange(2, skills.Count - 2);
        }
    }

    private void OnEnable()
    {
        InitializeStats();

        if (weapon != null)
            InitialWeapon();
    }

    private void Start()
    {
        _canReceivedDamage = true;

        // Suscribir a eventos de habilidades (máximo 2)
        ActionForControlPlayer.skillOne += () => LaunchedSkill(0);
        ActionForControlPlayer.skillTwo += () => LaunchedSkill(1);
    }

    /// <summary>
    /// Inicializa las estadísticas del jugador
    /// </summary>
    private void InitializeStats()
    {
        _generalStats = new float[7]; // 7 stats
        for (int i = 0; i < _generalMaxStats.Length && i < 7; i++)
        {
            _generalStats[i] = _generalMaxStats[i];
        }

        canLaunchAttack = true;
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

        if (_statsInUI != null && _statsInUI.dieUI != null)
        {
            _statsInUI.dieUI.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[PlayerStats] No se pudo mostrar la UI de muerte.");
        }
    }

    // ---- FUNCIONES BASE ---- //
    public void TakeDamage(GameObject obj, int dmg)
    {
        if (!_canReceivedDamage) return;
        if (dmg <= 0) return;

        // Intentar obtener el atacante para cálculos especiales
        EnemyManager attacker = obj?.GetComponent<EnemyManager>();
        if (attacker != null)
        {
            dmg = CalculateNewDamage(attacker, dmg);
        }

        if (dmg <= 0)
        {
            Debug.Log("[PlayerStats] Daño completamente prevenido");
            return;
        }

        // EVENTO = RECIBIR DAÑO
        takeDamage?.Invoke();

        // Aplicar el daño
        ApplyDamageInternal(dmg);

        // Aplicar efectos visuales y estado de intangibilidad
        ApplyDamageEffects();

        // Guardar daño recibido
        countDamageReceivedInRoom += dmg;

        // Verificar si sigue vivo
        if (_generalStats[0] <= 0)
        {
            StartCoroutine(Die());
        }
    }

    /// <summary>
    /// Aplica daño interno al jugador
    /// </summary>
    private void ApplyDamageInternal(int dmg)
    {
        SetValue(0, -dmg, false);

        if (_statsInUI != null)
        {
            _statsInUI.SetHUD(0, _generalStats[0], _generalMaxStats[0]);
        }
    }

    /// <summary>
    /// Aplica efectos visuales de daño
    /// </summary>
    private void ApplyDamageEffects()
    {
        _canReceivedDamage = false;

        if (_spr != null)
        {
            _spr.color = Color.red;
            Invoke(nameof(ResetColor), timeToTakeDamage);
        }

        Invoke(nameof(RemoveStateIntangible), timeToIntangible);
    }

    // ---- OBJECTS ---- //
    public void AddObject(Object obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[PlayerStats] Intentando agregar un objeto null");
            return;
        }

        objects.Add(obj);

        if (triggering != null)
        {
            triggering.SetObjects(objects);
        }

        if (_statsInUI != null)
        {
            _statsInUI.AddObjectInUI();
        }
    }

    // ---- SKILLS (Máximo 2) ---- //
    private void LaunchedSkill(int pos)
    {
        if (!skills.IsValidIndex(pos, "skills"))
            return;

        SkillManager skill = skills[pos];
        if (skill == null)
        {
            Debug.LogWarning($"[PlayerStats] Skill en posición {pos} es null");
            return;
        }

        bool canLaunch = CanLaunchSkill(pos, skill);

        if (canLaunch)
        {
            CreateSkill(pos);
        }
    }

    /// <summary>
    /// Verifica si se puede lanzar una habilidad según su tipo de carga
    /// </summary>
    private bool CanLaunchSkill(int pos, SkillManager skill)
    {
        switch (skill.loadType)
        {
            case LoadTypeSkill.concentration:
                return _generalStats.IsValidIndex(1) && _generalStats[1] >= skill.amountFuel;

            case LoadTypeSkill.kills:
                return countKillsInRoom >= skill.amountFuel;

            case LoadTypeSkill.receiveDamage:
                return countDamageReceivedInRoom >= skill.amountFuel;

            case LoadTypeSkill.damage:
                return countDamageInRoom >= skill.amountFuel;

            default:
                return false;
        }
    }

    private void CreateSkill(int id)
    {
        if (!skills.IsValidIndex(id, "skills"))
            return;

        SkillManager skill = skills[id];
        if (skill == null || skill.gameObject == null)
        {
            Debug.LogWarning($"[PlayerStats] No se puede crear skill en posición {id}");
            return;
        }

        Vector3 position = transform.position;

        if (skill.typeShow == TypeShowSkill.created)
        {
            // TODO: Crear encima del enemigo al que le apunta el jugador
        }

        LessAmountPerSkill(id);

        for(int i = 0; i < skill.countCreated; i++)
        {
            Instantiate(skill.gameObject, position, Quaternion.identity);
        }
    }

    private void LessAmountPerSkill(int pos)
    {
        if (!skills.IsValidIndex(pos, "skills"))
            return;

        SkillManager skill = skills[pos];
        if (skill == null) return;

        switch (skill.loadType)
        {
            case LoadTypeSkill.concentration:
                SetValue(1, -skill.amountFuel, false);
                break;

            case LoadTypeSkill.damage:
                countDamageInRoom -= skill.amountFuel;
                break;

            case LoadTypeSkill.receiveDamage:
                countDamageReceivedInRoom -= skill.amountFuel;
                break;
        }
    }

    // ---- SETTERS ---- //
    public void PreventDamagePerType(int[] count, bool[] reflects)
    {
        if (count != null && count.Length == 5)
            countPrevent = count;

        if (reflects != null && reflects.Length == 5)
            whichReflect = reflects;
    }

    public void PreventDamagePerDistance(int[] count, bool[] reflects)
    {
        if (count != null && count.Length == 2)
            preventDistance = count;

        if (reflects != null && reflects.Length == 2)
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

        if (!_generalStats.IsValidIndex(type, "_generalStats") ||
            !_generalMaxStats.IsValidIndex(type, "_generalMaxStats"))
        {
            Debug.LogWarning($"[PlayerStats] Índice de stat {type} inválido");
            return;
        }

        if (_generalStats[type] == _generalMaxStats[type] && !max && value > 0)
            return;

        // Mostrar texto flotante
        if (canShow)
        {
            string displayValue = value < 0 ? value.ToString() : ("+" + value.ToString());
            FloatTextManager.CreateText(transform.position, (TypeColor)type, displayValue);
        }

        // Aplicar el valor
        if (equalValue)
        {
            if (max) _generalMaxStats[type] = value;
            else _generalStats[type] = value;
        }
        else
        {
            if (max)
            {
                _generalMaxStats[type] += value;
            }
            else
            {
                // Comprobación necesaria para no pasarse del valor máximo de stats
                if ((_generalStats[type] + value) > _generalMaxStats[type])
                {
                    _generalStats[type] = _generalMaxStats[type];
                }
                else
                {
                    _generalStats[type] += value;
                }
            }
        }

        // Actualizar HUD
        UpdateHUD(type, max);
    }

    /// <summary>
    /// Actualiza la interfaz según el tipo de estadística modificada
    /// </summary>
    private void UpdateHUD(int type, bool max)
    {
        if (_statsInUI == null) return;

        // Concentración
        if (type == 1 && !max)
        {
            _statsInUI.SetHUD(1, _generalStats[1], _generalMaxStats[1]);
        }

        _statsInUI.ChangeValueInUI(type);
    }

    public float ChangerConcentration
    {
        set { changerConcentration = value; }
        get { return changerConcentration; }
    }

    // ---- WEAPON (Solo 1 slot) ---- //
    public int SetWeapon(WeaponSystem newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogWarning("[PlayerStats] Intentando establecer un arma null");
            return -1;
        }

        int previousWeaponID = -1;

        // Si ya hay un arma, destruirla
        if (weapon != null)
        {
            previousWeaponID = weapon.weaponID;

            if (weaponInScene != null)
            {
                Destroy(weaponInScene.gameObject, 1f);
                weaponInScene = null;
            }
        }

        weapon = newWeapon;
        CreateWeaponInScene();

        if (_statsInUI != null)
        {
            _statsInUI.SetWeaponInHUD(weapon.spr);
        }

        return previousWeaponID;
    }

    public void UpdateWeaponInAction()
    {
        weaponChanged?.Invoke();
    }

    // ---- ROOM STATS ---- //
    public void SetCountKills()
    {
        countKillsInRoom++;
    }

    public void SetCountDamage(int count)
    {
        countDamageInRoom += count;
    }

    public void ResetValuesPerRoom()
    {
        countKillsInRoom = 0;
        countDamageInRoom = 0;
        countDamageReceivedInRoom = 0;
    }

    public void LaunchAttack()
    {
        StartCoroutine(LaunchAttackDelay());
    }

    private IEnumerator LaunchAttackDelay()
    {
        canLaunchAttack = false;

        yield return new WaitForSeconds(delayToAttack);

        canLaunchAttack = true;
    }

    // ---- GETTERS ---- //
    public float GetterStats(int pos, bool max = true)
    {
        if (max)
        {
            return _generalMaxStats.GetSafeValue(pos, 0f);
        }
        else
        {
            return _generalStats.GetSafeValue(pos, 0f);
        }
    }

    // ---- FUNCION INTEGRA: DAMAGE CALCULATION ---- //
    private int CalculateNewDamage(EnemyManager attacker, int dmg)
    {
        if (attacker == null) return 0;

        // Verificar prevención y reflejo por tipo de ataque
        dmg = ApplyDamagePreventionByType(attacker, dmg);

        // Verificar prevención y reflejo por distancia
        dmg = ApplyDamagePreventionByDistance(attacker, dmg);

        return dmg;
    }

    /// <summary>
    /// Aplica la prevención de daño basada en el tipo de ataque del enemigo
    /// </summary>
    private int ApplyDamagePreventionByType(EnemyManager attacker, int dmg)
    {
        int typeIndex = GetDamageTypeIndex(attacker.typeAttack);

        if (typeIndex >= 0 && countPrevent.IsValidIndex(typeIndex))
        {
            if (whichReflect.IsValidIndex(typeIndex) && whichReflect[typeIndex])
            {
                attacker.TakeDamage(dmg);
            }

            int prevention = countPrevent[typeIndex];
            if (prevention > 0)
            {
                dmg -= (dmg * prevention / 100);
            }
        }

        return dmg;
    }

    /// <summary>
    /// Obtiene el índice del tipo de daño
    /// </summary>
    private int GetDamageTypeIndex(TypeEnemyAttack attackType)
    {
        switch (attackType)
        {
            case TypeEnemyAttack.Energy: return 1;
            case TypeEnemyAttack.Fire: return 2;
            case TypeEnemyAttack.Cold: return 3;
            case TypeEnemyAttack.Fortify: return 4;
            case TypeEnemyAttack.Base: return 0;
            default: return 0;
        }
    }

    /// <summary>
    /// Aplica la prevención de daño basada en la distancia del ataque
    /// </summary>
    private int ApplyDamagePreventionByDistance(EnemyManager attacker, int dmg)
    {
        int distanceIndex = attacker.distanceAttack ? 1 : 0;

        if (preventDistance.IsValidIndex(distanceIndex) && preventDistance[distanceIndex] > 0)
        {
            if (reflectDistance.IsValidIndex(distanceIndex) && reflectDistance[distanceIndex])
            {
                attacker.TakeDamage(dmg);
            }

            int prevention = preventDistance[distanceIndex];
            dmg -= (dmg * prevention / 100);
        }

        return dmg;
    }

    // ---- WEAPON MANAGEMENT (Solo 1 arma) ---- //
    private void InitialWeapon()
    {
        if (weapon == null) return;
        if (weaponParent == null)
        {
            Debug.LogError("[PlayerStats] weaponParent no está asignado. No se puede crear arma.");
            return;
        }

        WeaponSystem newWeapon = Instantiate(
            weapon.gameObject,
            weaponParent.transform.position,
            Quaternion.identity,
            weaponParent.transform
        ).GetComponent<WeaponSystem>();

        if (newWeapon != null)
        {
            weaponInScene = newWeapon;
        }

        weaponChanged?.Invoke();
    }

    private void CreateWeaponInScene()
    {
        if (weaponParent == null || weapon == null)
        {
            Debug.LogWarning($"[PlayerStats] No se puede crear arma");
            return;
        }

        WeaponSystem newWeapon = Instantiate(
            weapon.gameObject,
            weaponParent.transform.position,
            Quaternion.identity,
            weaponParent.transform
        ).GetComponent<WeaponSystem>();

        if (newWeapon != null)
        {
            weaponInScene = newWeapon;
        }

        weaponChanged?.Invoke();
    }

    private void ResetColor()
    {
        if (_spr != null)
            _spr.color = Color.white;
    }

    private void RemoveStateIntangible()
    {
        _canReceivedDamage = true;
    }

    // ---- FUNCION INTEGRA: WEAPON CHANCES ---- //
    public bool ComprobationCriticalChance()
    {
        if (!_generalStats.IsValidIndex(6))
            return false;

        int rnd = UnityEngine.Random.Range(0, 100);

        if (rnd < _generalStats[6])
        {
            criticalChance?.Invoke();
            FloatTextManager.CreateText(transform.position, TypeColor.CriticalChance, "48", false, true);
            return true;
        }

        return false;
    }
}
