using UnityEngine;

public class WeaponSystem : MonoBehaviour {

    [Header("Distance")]
    public bool distanceAttack;
    public GameObject projectile;
    public float distanceToAttack;
    public bool canTraverse;

    [Header("Attack System")]
    [HideInInspector] public int countAttack = 0;
    [HideInInspector] public bool canAttack = true;

    [Header("Modify Stats")]
    public float[] stats = new float[11];

    [Header("Timers")]
    [Range(0, 3), Tooltip("Tiempo entre ataques")] public float delayBetweenAttack;
    [HideInInspector] public float delayBetweenBase;
    private float delayDetect = 0.35f;

    [Header("Private Data")]
    private CrosshairData _crosshair;
    private PlayerStats _player;
    private Animator anim;

    private void Start()
    {
        _player = GetComponentInParent<PlayerStats>();
        _crosshair = FindAnyObjectByType<CrosshairData>();
        anim = _player.GetComponent<Animator>();

        delayBetweenBase = delayBetweenAttack;

        // MODIFICADOR DE STATS DE CADA WEAPON
        for(int i = 0; i < stats.Length; i++)
        {
            _player.SetValue(i, stats[i], false, false);
            _player.SetValue(i, stats[i], true, false);
        }

        PlayerStats.jumpBetween += () => countAttack = 0;
    }
    private void Update()
    {
        delayDetect -= Time.deltaTime;
        if (delayDetect <= 0) countAttack = 0;

        if (!canAttack)
        {
            delayBetweenAttack -= Time.deltaTime;

            if(delayBetweenAttack <= 0) canAttack = true;
        }
    }
    public void Attack()
    {
        if (!canAttack) return;

        // FUNCIONAMIENTO DE DELAY ENTRE ATAQUES
        delayBetweenAttack = delayBetweenBase;
        delayDetect = 0.35f;
        canAttack = false;

        countAttack++;
        anim.SetBool("Attack", true);

        if (distanceAttack)
        {
            CreateProjectile();
            return;
        }
    }
    private void CreateProjectile()
    {
        // ATAQUE A DISTANCIA
        Projectile pr = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();

        // VERIFICAR DIFERENCIA ENTRE DAÑO-1 Y DAÑO SUCESIVO
        int damage = (int)_player.GetterStats(5, false);
        if (countAttack > 2) damage = (int)(_player.GetterStats(5, false) * (_player.GetterStats(6, false) / 100));

        pr.SetterValues(gameObject, distanceToAttack, damage, canTraverse, _crosshair.GetCurrentAimDirection());
    }
    // ---- EVENTO DEL ANIMATOR ---- //
    public void MeleeAttack() { gameObject.tag = "Projectile"; }
    public void FinishAttack()
    {
        gameObject.tag = "Weapon";
        anim.SetBool("Attack", false);
    }
}
