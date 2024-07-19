using System.Collections;
using UnityEngine;

public abstract class WeaponSystem : MonoBehaviour {

    [Header("Info General")]
    public int weaponID;
    public Sprite spr;
    public int nameWeapon;
    public int descWeapon;
    public int damageWeapon;
    public int damageFinalHit;
    [HideInInspector] public int damage;

    [Header("Attack System")]
    [HideInInspector] public int countAttack = 0;
    [HideInInspector] public bool canAttack = true;

    [Header("Timers")]
    [Range(0, 3), Tooltip("Tiempo entre ataques")] public float delayBetweenAttack;
    [Range(0, 3), Tooltip("Tiempo que tarda en dejar de detectar un combo")] public float delayDetect = 1.75f;
    [Range(0, 2), Tooltip("Duración del ataque")] public float durationDamage = 0.35f;
    [Range(0, 2), Tooltip("Tiempo que tarda en dejar de detectar que acabas de saltar (roll)")] public float delayDetectRoll = 0.65f;
    private float _durationDamageBase;
    private float _delayBetweenBase;
    private float _delayDetectBase;
    private float _delayBaseRoll;
    private bool _inAttack = false;
    private bool _damageSubsequence = false;

    [Header("Private Data")]
    protected PlayerStats _playerStats;
    private Animator anim;

    private void OnEnable()
    {
        _playerStats = GetComponentInParent<PlayerStats>();
        anim = _playerStats.GetComponent<Animator>();

        _delayBetweenBase = delayBetweenAttack;
        _durationDamageBase = durationDamage;
        _delayBaseRoll = delayDetectRoll;
        _delayDetectBase = delayDetect;
        delayDetectRoll = 0;

        PlayerStats.jumpBetween += DelayMadeJump;
    }
    private void Update()
    {
        delayDetect -= Time.deltaTime;
        if (delayDetect <= 0)
        {
            countAttack = 0;
            delayDetect = _delayDetectBase;
        }

        if(delayDetectRoll > 0)
        {
            delayDetectRoll -= Time.deltaTime;
        }else { _damageSubsequence = false; }

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
    private void DelayMadeJump()
    {
        delayDetectRoll = _delayBaseRoll;
        _damageSubsequence = true;
    }
    public void PrepareAttack()
    {
        if (!canAttack) return;

        // FUNCIONAMIENTO DE DELAY ENTRE ATAQUES
        delayBetweenAttack = _delayBetweenBase;
        delayDetect = _delayDetectBase;

        countAttack++;
        anim.SetBool("Attack", true);
        _inAttack = true;

        bool missChance = _playerStats.ComprobationMissChance();
        bool criticalChance = _playerStats.ComprobationCriticalChance();

        if(countAttack >= 3)
        {
            damage = damageFinalHit;
            delayBetweenAttack = (_delayBetweenBase * 1.5f);
            countAttack = 0;

            if (missChance) { damage = 0; }
            if (criticalChance) { damage *= 2; }

            if (_damageSubsequence) damage += (int)(_playerStats.GetterStats(6, true) * damage / 100);
            
            FinalHit();
        }
        else
        {
            damage = damageWeapon;

            if (missChance) damage = 0;
            if (criticalChance) damage *= 2;

            if (_damageSubsequence) damage += (int)(_playerStats.GetterStats(6, true) * damage / 100);

            Attack();
        }

        canAttack = false;
    }
    public abstract void Attack();
    public abstract void FinalHit();
    // ---- EVENTO DEL ANIMATOR ---- //
    public void FinishAttack()
    {
        anim.SetBool("Attack", false);

        gameObject.tag = "Weapon";
        _inAttack = false;
    }
    // ---- DESTROYED ---- //
    private void OnDisable() { PlayerStats.jumpBetween -= DelayMadeJump; }
    private void OnDestroy() { PlayerStats.jumpBetween -= DelayMadeJump; }
}