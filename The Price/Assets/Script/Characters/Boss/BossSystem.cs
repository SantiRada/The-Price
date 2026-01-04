using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeBoss { minBoss, Boss, maxBoss }

public class BossSystem : EnemyBase
{
    [Header("Boss Configuration")]
    [Tooltip("ScriptableObject con toda la configuraci�n del boss (RECOMENDADO)")]
    public BossData bossData;

    [Header("Manual Configuration (Legacy - usar solo si no hay BossData)")]
    [Tooltip("Cantidad total de fases que tendr� el boss durante el combate")]
    public int countPhase;

    [Tooltip("Este BOSS cambia de fase por cantidad de vida (o por tiempo).")]
    public bool changePhasePerLife;

    [Tooltip("Cantidad de vida o tiempo para cambiar de fase.")]
    public List<int> limiterPerPhase = new List<int>();

    [Tooltip("Cantidad de ataques que hace en cada fase.")]
    public List<int> countAttacksPerPhase = new List<int>();

    [Tooltip("Lista total de Ataques.")]
    public List<AttackBoss> attacks = new List<AttackBoss>();

    private int indexPhase = 0;

    [Header("General Stats")]
    [Tooltip("Identificador del boss para l�gica de guardado u otros sistemas")]
    public int nameBoss;

    [Tooltip("Unicamente sirve para el JSON de guardado de muertes.")]
    public TypeBoss typeBoss;

    [Header("Move & Attack")]
    [Tooltip("Si lo es, tiene m�s probabilidad de atacar que de moverse por el mapa.")]
    public bool isAggressive;

    [Tooltip("Si es TRUE, en fases posteriores incluye todos los ataques de fases previas.")]
    public bool includeAllAttacks;

    [Space]
    private float maxDistance, minDistance;
    private int indexMin;

    [Header("Private Content")]
    private BossUI _bossUI;
    [Space]
    private List<TypeMovement> _typeMovement = new List<TypeMovement>();
    private List<AttackBoss> _typeAttacks = new List<AttackBoss>();

    private void Start()
    {
        // Cargar configuración desde BossData si está disponible
        if (bossData != null)
        {
            LoadFromBossData();
        }

        _bossUI = FindAnyObjectByType<BossUI>();
        _typeMovement.AddRange(GetComponents<TypeMovement>());

        // Instanciar patrones de movimiento desde BossData si están definidos
        if (bossData != null && bossData.movementPatternPrefabs.Count > 0)
        {
            foreach (GameObject movementPrefab in bossData.movementPatternPrefabs)
            {
                GameObject movementObj = Instantiate(movementPrefab, transform);
                TypeMovement movement = movementObj.GetComponent<TypeMovement>();
                if (movement != null)
                {
                    _typeMovement.Add(movement);
                }
            }
        }

        // Instancia los ataques definidos y los asocia al boss
        List<AttackBoss> attacksToInstantiate = (bossData != null && bossData.attackPrefabs.Count > 0)
            ? ConvertAttackPrefabsToAttackBoss(bossData.attackPrefabs)
            : attacks;

        for (int i = 0; i < attacksToInstantiate.Count; i++)
        {
            _typeAttacks.Add(Instantiate(attacksToInstantiate[i].gameObject, transform.position, Quaternion.identity, transform).GetComponent<AttackBoss>());
        }

        ComprobateDistanceToPlayer();
        StartCoroutine("Presentation");
    }

    /// <summary>
    /// Carga la configuración del boss desde el ScriptableObject BossData
    /// </summary>
    private void LoadFromBossData()
    {
        // Identificación
        nameBoss = bossData.bossID;
        typeBoss = bossData.bossType;

        // Stats base
        health = bossData.maxHealth;
        shield = bossData.initialShield;
        damage = bossData.baseDamage;
        speed = bossData.baseSpeed;
        defense = bossData.defense;

        // Sistema de fases
        changePhasePerLife = bossData.changePhaseByHealth;
        countPhase = bossData.GetPhaseCount();

        limiterPerPhase.Clear();
        countAttacksPerPhase.Clear();

        for (int i = 0; i < bossData.phases.Count; i++)
        {
            limiterPerPhase.Add(bossData.phases[i].triggerValue);
            countAttacksPerPhase.Add(bossData.phases[i].attackCount);
        }

        // Distancias de combate
        maxDistance = bossData.maxCombatDistance;
        minDistance = bossData.minCombatDistance;

        // Comportamiento de ataque (usar primera fase como referencia)
        if (bossData.phases.Count > 0)
        {
            isAggressive = bossData.phases[0].aggressionLevel > 50;
            includeAllAttacks = bossData.phases[0].includeAllPreviousAttacks;
        }
    }

    /// <summary>
    /// Convierte los prefabs de ataque de BossData en lista de AttackBoss
    /// </summary>
    private List<AttackBoss> ConvertAttackPrefabsToAttackBoss(List<GameObject> prefabs)
    {
        List<AttackBoss> result = new List<AttackBoss>();
        foreach (GameObject prefab in prefabs)
        {
            AttackBoss attack = prefab.GetComponent<AttackBoss>();
            if (attack != null)
            {
                result.Add(attack);
            }
        }
        return result;
    }
    private void Update()
    {
        if (Pause.state != State.Game || LoadingScreen.inLoading) return;

        if (canMove)
        {
            if (!canAttack)
            {
                Movement();
                return;
            }

            if (Vector3.Distance(transform.position, _playerStats.transform.position) > maxDistance) { Movement(); }
            else if (Vector3.Distance(transform.position, _playerStats.transform.position) < minDistance) { Attack(indexMin); }
            else
            {
                int rnd = Random.Range(0, 100);

                if (isAggressive)
                {
                    if (rnd < 70) { Attack(); }
                    else { Movement(); }
                }
                else
                {
                    if (rnd < 70) { Movement(); }
                    else { Attack(); }
                }
            }
        }
        else if (canAttack) { Attack(); }
    }
    // ---- STOPPERS ---- //
    private IEnumerator ChangePhase()
    {
        CancelEnemy(true);
        if ((countAttacksPerPhase.Count - 1) > indexPhase) indexPhase++;

        // Aplicar modificadores de la nueva fase desde BossData
        if (bossData != null && indexPhase < bossData.phases.Count)
        {
            ApplyPhaseModifiers(indexPhase);
        }

        // anim.SetBool("ChangePhase", true);

        yield return new WaitForSeconds(2f);

        // anim.SetBool("ChangePhase", false);

        CancelEnemy(true);
    }

    /// <summary>
    /// Aplica los modificadores de stats de una fase específica
    /// </summary>
    private void ApplyPhaseModifiers(int phaseIndex)
    {
        BossPhaseData phaseData = bossData.GetPhaseData(phaseIndex);
        if (phaseData == null) return;

        // Aplicar multiplicadores de stats
        damage = Mathf.RoundToInt(bossData.baseDamage * phaseData.damageMultiplier);
        speed = bossData.baseSpeed * phaseData.speedMultiplier;

        // Actualizar comportamiento de agresión
        isAggressive = phaseData.aggressionLevel > 50;

        // Instanciar efecto visual de cambio de fase si existe
        if (phaseData.phaseChangeEffect != null)
        {
            Instantiate(phaseData.phaseChangeEffect, transform.position, Quaternion.identity);
        }

        // Actualizar color de la UI si está disponible
        if (_bossUI != null)
        {
            // Aquí se podría actualizar el color de la barra de vida
            // _bossUI.SetHealthBarColor(phaseData.phaseColor);
        }
    }
    private IEnumerator Presentation()
    {
        yield return new WaitForSeconds(2f);

        // INICIA LA UI DEL BOSS
        _bossUI.StartUIPerBoss(nameBoss, health, shield);

        indexPhase = 0;
        CancelEnemy(false);
        Pause.StateChange = State.Interface;
        CameraMovement.CallCamera(transform.position, 2f);

        // anim.SetBool("Presentation", true);

        yield return new WaitForSeconds(3.5f);

        // anim.SetBool("Presentation", false);

        Pause.StateChange = State.Game;
        CancelEnemy(true);
    }
    // ---- REPEATERS ---- //
    private void Movement()
    {
        if (inAttack || inMove) return;

        int rnd = Random.Range(0, _typeMovement.Count);

        canMove = false;
        inMove = true;

        _typeMovement[rnd].Move();
    }
    // ---- FUNCION INTEGRA ---- //
    private void ComprobateDistanceToPlayer()
    {
        minDistance = 1000;
        maxDistance = 0;

        for (int i = 0; i < _typeAttacks.Count; i++)
        {
            // GUARDA EL ATAQUE DE M�NIMA DISTANCIA
            if (_typeAttacks[i].distanceToAttack < minDistance)
            {
                minDistance = _typeAttacks[i].distanceToAttack;
                indexMin = i;
            }

            // GUARDA EL ATAQUE DE M�XIMA DISTANCIA
            if (_typeAttacks[i].distanceToAttack > maxDistance) { maxDistance = _typeAttacks[i].distanceToAttack; }
        }
    }
    // ---- OVERRIDE ---- //
    public override IEnumerator Die()
    {
        CancelEnemy(true);
        Pause.StateChange = State.Interface;

        StartCoroutine("CancelMove");
        StartCoroutine("CancelAttack");

        yield return new WaitForSeconds(0.5f);

        CameraMovement.CallCamera(transform.position, 3f);

        // anim.SetBool("Die", true);

        yield return new WaitForSeconds(3.5f);

        Pause.StateChange = State.Interface;
        _room.Advance();

        _bossUI.StartCoroutine("HideUI");

        Destroy(gameObject);
    }
    public override void SpecificMove()
    {
        for (int i = 0; i < _typeMovement.Count; i++)
        {
            _typeMovement[i].CancelMove();
        }
    }
    public override void SpecificTakeDamage(int dmg)
    {
        // APLICAR CAMBIOS EN LA UI
        _bossUI.SetStats(health, shield);

        if (health <= limiterPerPhase[indexPhase])
        {
            if (indexPhase < countPhase) StartCoroutine("ChangePhase");
        }
    }
    public override void SpecificAttack(int index = -1)
    {
        #region CalculateTypeAttack
        int minValue;

        if (includeAllAttacks) minValue = 0;
        else minValue = (indexPhase > 0 ? countAttacksPerPhase[(indexPhase - 1)] : 0);

        int rnd = Random.Range(minValue, countAttacksPerPhase[indexPhase]);

        if (index != -1) rnd = index;
        #endregion

        _typeAttacks[rnd].enemyParent = this;
        _typeAttacks[rnd].StartCoroutine("Attack");
    }
    public override void SpecificState(TypeState state, int numberOfLoads) { Debug.Log("Not Specific"); }
}
