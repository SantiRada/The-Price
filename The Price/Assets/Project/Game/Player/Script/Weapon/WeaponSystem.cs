using Unity.VisualScripting;
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
    private float delayDetect = 1.75f;

    [Header("Private Data")]
    private CrosshairData _crosshair;
    private PlayerStats _player;
    private Animator anim;

    private void OnEnable()
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
    // ---- COMBO ---- //
    public void FinalAttack(int combo)
    {
        // if (countAttack <= 2) return;

        Debug.Log("Ha tirao un combo with " + combo);

        switch (combo)
        {
            case 0: CreateProjectile(0.35f, 3); break;
            case 1: CreateProjectile(1.25f); break;
            case 2: CreateProjectile(0.35f, 6); break;
        }

        countAttack = 0;
    }
    public void Attack()
    {
        if (!canAttack) return;

        // FUNCIONAMIENTO DE DELAY ENTRE ATAQUES
        delayBetweenAttack = delayBetweenBase;
        delayDetect = 1.75f;
        canAttack = false;

        countAttack++;
        if (countAttack >= 3)
        {
            FinalAttack(0);
            return;
        }

        anim.SetBool("Attack", true);

        if (distanceAttack)
        {
            CreateProjectile();
            return;
        }
    }
    private void CreateProjectile(float size = 0.35f, int count = 1)
    {
        Vector3 target = _crosshair.GetCurrentAimDirection();

        for (int i = 0; i < count; i++)
        {
            // ATAQUE A DISTANCIA
            Projectile pr = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();

            pr.transform.localScale = new Vector3(size, size, size);

            // VERIFICAR DIFERENCIA ENTRE DAÑO-1 Y DAÑO SUCESIVO
            int damage = (int)_player.GetterStats(5, false);
            if (countAttack > 2) damage = (int)(_player.GetterStats(5, false) * (_player.GetterStats(6, false) / 100));

            pr.SetterValues(gameObject, distanceToAttack, damage, canTraverse, target);
        }
    }
    // ---- EVENTO DEL ANIMATOR ---- //
    public void MeleeAttack() { gameObject.tag = "Projectile"; }
    public void FinishAttack()
    {
        gameObject.tag = "Weapon";
        anim.SetBool("Attack", false);
    }
}
