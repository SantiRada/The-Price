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
    [Range(0, 3), Tooltip("Tiempo que tarda en dejarde detectar un combo")] public float delayDetect = 1.75f;
    [Range(0, 2), Tooltip("Duración del ataque")] public float durationDamage = 0.35f;
    private float _durationDamageBase;
    private float _delayBetweenBase;
    private float _delayDetectBase;
    private bool _inAttack = false;

    [Header("Private Data")]
    protected PlayerStats _player;
    private Animator anim;

    private void OnEnable()
    {
        _player = GetComponentInParent<PlayerStats>();
        anim = _player.GetComponent<Animator>();

        _delayBetweenBase = delayBetweenAttack;
        _durationDamageBase = durationDamage;
        _delayDetectBase = delayDetect;
    }
    private void Update()
    {
        delayDetect -= Time.deltaTime;
        if (delayDetect <= 0)
        {
            countAttack = 0;
            delayDetect = _delayDetectBase;
        }

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

        // FUNCIONAMIENTO DE DELAY ENTRE ATAQUES
        delayBetweenAttack = _delayBetweenBase;
        delayDetect = _delayDetectBase;

        countAttack++;
        anim.SetBool("Attack", true);
        _inAttack = true;

        if(countAttack >= 3)
        {
            damage = damageFinalHit;
            delayBetweenAttack = (_delayBetweenBase * 1.5f);
            countAttack = 0;
            FinalHit();
        }
        else
        {
            damage = damageWeapon;
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

        _player.CanReceivedDamage = true;
    }
}