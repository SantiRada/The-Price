using UnityEngine;

public class WeaponSystem : MonoBehaviour {

    [Header("Distance")]
    public bool distanceAttack;
    public GameObject projectile;
    public float distanceToAttack;
    public bool canTraverse;

    [Header("Attack System")]
    public bool hasCombo;
    public int countAttackPerCombo;
    [HideInInspector] public int countAttack = 0;
    [HideInInspector] public bool canAttack = true;

    [Header("Timers")]
    [Range(0, 3), Tooltip("Tiempo de detección entre ataques para generar un Combo")] public float delayDetectionAttack;
    [Range(0, 3), Tooltip("Tiempo entre ataques")] public float delayBetweenAttack;
    [HideInInspector] public float delayDetectionBase, delayBetweenBase;

    [Header("Private Data")]
    private CrosshairData _crosshair;
    private PlayerStats _stats;
    private Animator anim;

    private void Start()
    {
        _stats = GetComponentInParent<PlayerStats>();
        _crosshair = FindAnyObjectByType<CrosshairData>();

        anim = _stats.GetComponent<Animator>();
    }
    private void Update()
    {
        if (!canAttack)
        {
            delayBetweenAttack -= Time.deltaTime;

            if(delayBetweenAttack <= 0) canAttack = true;
        }

        // FUNCIONAMIENTO DEL COMBO BASE
        delayDetectionAttack -= Time.deltaTime;

        if (delayDetectionAttack <= 0) countAttack = 0;

        if(countAttack >= countAttackPerCombo) Combo();
    }
    private void Combo()
    {
        Debug.Log("Se ha activado el Combo");
    }
    public void Attack()
    {
        if (!canAttack) return;

        // FUNCIONAMIENTO DEL DELAY PARA MANEJO DE COMBO
        countAttack++;
        delayDetectionAttack = delayDetectionBase;

        // FUNCIONAMIENTO DE DELAY ENTRE ATAQUES
        delayBetweenAttack = delayBetweenBase;
        canAttack = false;

        if (distanceAttack)
        {
            CreateProjectile();
            return;
        }

        anim.SetBool("Attack", true);
    }
    private void CreateProjectile()
    {
        Debug.Log("Create Projectile");
        // ATAQUE A DISTANCIA
        Projectile pr = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();
        pr.SetterValues(distanceToAttack, (int)_stats.GetterStats(5, false), canTraverse, _crosshair.GetCurrentAimDirection());
    }
    // ---- EVENTO DEL ANIMATOR ---- //
    public void MeleeAttack() { gameObject.tag = "Projectile"; }
    public void FinishAttack() { gameObject.tag = "Weapon"; }
}
