using System.Collections;
using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los enemigos del juego.
/// Maneja estadísticas, daño, movimiento y ataques de manera autosustentable.
/// Refactorizado con validaciones completas.
/// </summary>
public abstract class EnemyBase : MonoBehaviour {

    [Header("General Stats")]
    public int healthMax;
    public int shieldMax;
    public int damage;
    public float speed;
    public float sanity;
    private float sanityBase;
    [HideInInspector] public int health, shield;

    [Header("Collision")]
    [Range(0, 1), Tooltip("Tiempo en que no detecta daño después de haberlo recibido (intangibilidad)")]
    public float delayToDetectDamage;

    [Header("Rewards")]
    public bool canReleaseSouls;
    public int countSouls;
    [Space]
    public bool canReleaseGold;
    public CountGold countGold;

    [Header("Attack Data")]
    public float durationForEffect;
    public float delayBetweenAttack;
    public float delayBetweenMovement;
    protected bool inMove, canMove;
    protected bool canTakeDamage, inAttack, canAttack;

    [Header("Private Content")]
    protected PlayerStats _playerStats;
    protected Room _room;

    private void Awake()
    {
        InitializeComponents();
        InitialValues();
    }

    /// <summary>
    /// Inicializa y valida componentes necesarios
    /// </summary>
    private void InitializeComponents()
    {
        // Buscar PlayerStats - crítico para el funcionamiento
        if (!ComponentHelper.TryFindObjectSafe(out _playerStats, $"EnemyBase ({gameObject.name})"))
        {
            Debug.LogError($"[EnemyBase] PlayerStats no encontrado. El enemigo {gameObject.name} no podrá funcionar correctamente.");
        }

        // Buscar Room - puede no existir en escenas de prueba
        if (!ComponentHelper.TryFindObjectQuiet(out _room))
        {
            Debug.LogWarning($"[EnemyBase] Room no encontrado para {gameObject.name}. El sistema de spawning puede no funcionar.");
        }
    }

    /// <summary>
    /// Inicializa los valores del enemigo
    /// </summary>
    private void InitialValues()
    {
        sanityBase = sanity;
        health = healthMax;
        shield = shieldMax;

        CancelEnemy(false);
    }

    private void LateUpdate()
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

        UpdateSanity();
    }

    /// <summary>
    /// Actualiza el sistema de cordura/sanidad
    /// </summary>
    private void UpdateSanity()
    {
        if (sanity < sanityBase)
        {
            sanity += Time.deltaTime;
        }

        if (sanity <= 0)
        {
            StartCoroutine(ApplyEffect());
        }
    }

    /// <summary>
    /// Aplica el efecto de aturdimiento cuando la sanidad llega a 0
    /// </summary>
    private IEnumerator ApplyEffect()
    {
        // Se aplicó el efecto de stun
        sanity = sanityBase * 1.5f;

        inMove = false;
        canMove = false;
        inAttack = false;
        canAttack = false;

        yield return new WaitForSeconds(durationForEffect);

        canMove = true;
        canAttack = true;
    }

    // ---- ABSTRACTS ---- //
    public abstract IEnumerator Die();
    public abstract void SpecificMove();
    public abstract void SpecificAttack(int index);
    public abstract void SpecificTakeDamage(int dmg);
    public abstract void SpecificState(TypeState state, int numberOfLoads);

    // ---- MODIFICATORS ---- //
    public void AddState(TypeState state, int numberOfLoads)
    {
        SpecificState(state, numberOfLoads);

        AffectedState st = gameObject.AddComponent<AffectedState>();
        if (st != null)
        {
            st.CreateState(state, numberOfLoads);
        }
        else
        {
            Debug.LogWarning($"[EnemyBase] No se pudo agregar AffectedState a {gameObject.name}");
        }
    }

    // ---- CANCELS ---- //
    protected void CancelEnemy(bool value = false)
    {
        inMove = false;
        canMove = value;
        inAttack = false;
        canAttack = value;
        canTakeDamage = value;
    }

    public IEnumerator CancelMove()
    {
        SpecificMove();

        inMove = false;

        yield return new WaitForSeconds(delayBetweenMovement);

        if (health > 0) canMove = true;
    }

    public IEnumerator CancelAttack()
    {
        inAttack = false;

        yield return new WaitForSeconds(delayBetweenAttack);

        if (health > 0) canAttack = true;
    }

    // ---- CALLERS ---- //
    public IEnumerator TakeDamage(int dmg)
    {
        if (!canTakeDamage) yield break;

        canTakeDamage = false;

        // Aplicar daño al escudo primero
        if (shield >= dmg)
        {
            shield -= dmg;
        }
        else
        {
            dmg -= shield;
            shield = 0;

            if (health >= dmg)
                health -= dmg;
            else
                health = 0;
        }

        sanity -= 1;

        // Notificar al jugador del daño realizado
        if (_playerStats != null)
        {
            _playerStats.SetCountDamage(dmg);
        }

        SpecificTakeDamage(dmg);

        yield return new WaitForSeconds(delayToDetectDamage);

        canTakeDamage = true;

        // Si murió, soltar recompensas y ejecutar muerte
        if (health <= 0)
        {
            ReleaseRewards();
            StartCoroutine(Die());
        }
    }

    /// <summary>
    /// Suelta las recompensas del enemigo al morir
    /// </summary>
    private void ReleaseRewards()
    {
        Vector3 dropPosition = transform.position + new Vector3(0.5f, 0.5f, 0);

        if (canReleaseSouls)
        {
            ManagerGold.CreateSouls(dropPosition, countSouls);
        }

        if (canReleaseGold)
        {
            ManagerGold.CreateGold(transform.position + Vector3.one, countGold);
        }
    }

    public void Attack(int index = -1)
    {
        if (inAttack) return;

        if (inMove)
        {
            StartCoroutine(CancelMove());
        }

        SpecificAttack(index);

        canAttack = false;
        inAttack = true;
    }

    // ---- TRIGGERS ---- //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

        ProcessProjectileCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;

        // Solo procesar armas que persisten
        if (collision.HasTag(GameTags.Projectile))
        {
            if (collision.TryGetComponentSafe(out WeaponSystem weapon))
            {
                ProcessWeaponDamage(weapon);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LoadingScreen.inLoading || Pause.state != State.Game) return;
    }

    /// <summary>
    /// Procesa la colisión con proyectiles y aplica el daño correspondiente
    /// </summary>
    private void ProcessProjectileCollision(Collider2D collision)
    {
        if (!collision.HasTag(GameTags.Projectile)) return;

        int dmg = 0;
        bool shouldDestroy = false;

        // Verificar si es un arma
        if (collision.TryGetComponentSafe(out WeaponSystem weapon))
        {
            dmg = weapon.damage;
            shouldDestroy = weapon.destroyToDetectCollision;

            if (shouldDestroy)
            {
                weapon.FinishAttack();
            }
        }
        // Verificar si es un proyectil
        else if (collision.TryGetComponentSafe(out Projectile projectile))
        {
            // Verificar que el proyectil haya sido lanzado por el jugador
            if (projectile.whoIsBoss != 0) return;

            dmg = projectile.damage;

            // Destruir el proyectil si no puede atravesar objetos
            if (!projectile.canTraverse)
            {
                collision.gameObject.DestroySafe();
            }
        }

        if (dmg > 0)
        {
            StartCoroutine(TakeDamage(dmg));
        }
    }

    /// <summary>
    /// Procesa el daño de armas que persisten en el trigger
    /// </summary>
    private void ProcessWeaponDamage(WeaponSystem weapon)
    {
        if (weapon == null) return;

        int dmg = weapon.damage;

        if (weapon.destroyToDetectCollision)
        {
            weapon.FinishAttack();
        }

        StartCoroutine(TakeDamage(dmg));
    }

    // ---- GETTERS && SETTERS ---- //
    public bool CanAttack
    {
        set { canAttack = value; }
        get { return canAttack; }
    }

    public bool CanMove
    {
        set { canMove = value; }
        get { return canMove; }
    }

    public Room CurrentRoom
    {
        set
        {
            if (value != null)
                _room = value;
        }
        get { return _room; }
    }

    public bool InAttack
    {
        get { return inAttack; }
    }

    public bool InMove
    {
        get { return inMove; }
    }

    /// <summary>
    /// Verifica si el enemigo está vivo
    /// </summary>
    public bool IsAlive
    {
        get { return health > 0; }
    }
}
