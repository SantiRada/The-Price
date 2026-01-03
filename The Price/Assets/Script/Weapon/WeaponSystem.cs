using UnityEngine;

/// <summary>
/// Sistema base para todas las armas del jugador.
/// Simplificado para soportar solo armas a distancia.
/// </summary>
public abstract class WeaponSystem : MonoBehaviour {

    [Header("Info General")]
    public int weaponID;
    public Sprite spr;
    public int nameWeapon;
    public int descWeapon;
    public int damageWeapon;
    [HideInInspector] public int damage;
    public bool isStaticAttack;

    [Header("Attack System")]
    [HideInInspector] public bool canAttack = true;

    [Header("Timers")]
    public bool destroyToDetectCollision = false;
    [Range(0, 3), Tooltip("Tiempo entre ataques")] public float delayBetweenAttack;
    [Range(0, 2), Tooltip("Duración del ataque")] public float durationDamage = 0.35f;
    private float _durationDamageBase;
    private float _delayBetweenBase;
    private bool _inAttack = false;

    [Header("Private Data")]
    protected PlayerStats _playerStats;
    protected Animator anim;
    private ActionForControlPlayer _actionUI;
    private PlayerMovement _movement;

    private void Awake()
    {
        if (this.TryGetComponentSafe(out anim))
        {
            // Animator encontrado
        }
    }

    private void OnEnable()
    {
        _playerStats = GetComponentInParent<PlayerStats>();

        if (_playerStats != null)
        {
            if (_playerStats.TryGetComponentSafe(out _movement))
            {
                // Movement encontrado
            }

            if (_playerStats.TryGetComponentSafe(out _actionUI))
            {
                _actionUI.changeRotation += CalculateRotation;
            }
        }

        _delayBetweenBase = delayBetweenAttack;
        _durationDamageBase = durationDamage;

        CalculateRotation();
    }

    private void Update()
    {
        if (!canAttack)
        {
            delayBetweenAttack -= Time.deltaTime;

            if(delayBetweenAttack <= 0) canAttack = true;
        }

        if (_inAttack)
        {
            durationDamage -= Time.deltaTime;

            if (durationDamage <= 0)
            {
                durationDamage = _durationDamageBase;
                FinishAttack();
            }
        }
    }

    public void PrepareAttack()
    {
        if (!canAttack) return;
        if (_playerStats == null || !_playerStats.canLaunchAttack) return;

        _playerStats.LaunchAttack();

        // FUNCIONAMIENTO DE DELAY ENTRE ATAQUES
        delayBetweenAttack = _delayBetweenBase;

        if(isStaticAttack && _movement != null)
        {
            _movement.SetMove(false);
        }

        if (anim != null)
        {
            anim.SetBool("Attack", true);
        }

        _inAttack = true;

        // Calcular daño con probabilidad de crítico
        damage = damageWeapon;

        bool criticalChance = _playerStats != null && _playerStats.ComprobationCriticalChance();
        if (criticalChance)
        {
            damage *= 2;
        }

        Attack();
        canAttack = false;
    }

    private void CalculateRotation()
    {
        // Implementación específica en clases derivadas si es necesario
    }

    public abstract void Attack();

    // ---- EVENTO DEL ANIMATOR ---- //
    public void FinishAttack()
    {
        if (anim != null)
        {
            anim.SetBool("Attack", false);
        }

        if(isStaticAttack && _movement != null)
        {
            _movement.SetMove(true);
        }

        gameObject.tag = GameTags.Weapon;
        _inAttack = false;
    }

    // ---- DESTROYED ---- //
    private void OnDisable()
    {
        if (_actionUI != null)
        {
            _actionUI.changeRotation -= CalculateRotation;
        }
    }

    private void OnDestroy()
    {
        if (_actionUI != null)
        {
            _actionUI.changeRotation -= CalculateRotation;
        }
    }
}
