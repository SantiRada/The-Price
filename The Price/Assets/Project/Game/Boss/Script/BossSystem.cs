using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeBoss { minBoss, Boss, maxBoss }
public class BossSystem : MonoBehaviour {

    [Header("Manager Content")]
    public int countPhase;
    [Tooltip("Este BOSS cambia de fase por cantidad de vida (o por tiempo)")] public bool changePhasePerLife;
    [Tooltip("Cantidad de vida o tiempo para cambiar de fase")] public List<int> limiterPerPhase = new List<int>();
    [Tooltip("Cantidad de ataques que hace en cada fase")] public List<int> countAttacksPerPhase = new List<int>();
    public List<AttackBoss> attacks = new List<AttackBoss>();
    private int indexPhase = 0;

    [Header("Stats")]
    public int nameBoss;
    [Tooltip("Unicamente sirve para el JSON de guardado de muertes")] public TypeBoss typeBoss;
    public int health;
    public int shield;
    public int damageMultiplier;
    public float speed;
    public float sanity;
    [Tooltip("Tiempo que tenes que colisionar para que te aplique daño")] public float timeToDetectCollision;
    private float detectCollisionBase;
    private float sanityBase;

    [Header("Move & Attack")]
    public float timeBetweenAttacks;
    public float timeBetweenMovement;
    private float timerMovementBase;
    private float timerAttackBase;
    [Space]
    public bool inMove;
    public bool canMove;
    public bool inAttack;
    public bool canAttack;
    public bool canTakeDamage;
    public bool canInitial = false;
    [Space]
    private List<TypeMovement> _typeMovement = new List<TypeMovement>();
    private List<AttackBoss> _typeAttacks = new List<AttackBoss>();

    [Header("Private Content")]
    private PlayerStats _playerStats;
    private Room _room;
    private BossUI _bossUI;

    private void OnEnable()
    {
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _bossUI = FindAnyObjectByType<BossUI>();
        _room = FindAnyObjectByType<Room>();

        _typeMovement.AddRange(GetComponents<TypeMovement>());
    }
    private void Start()
    {
        // TIMERS INICIALES
        detectCollisionBase = timeToDetectCollision;
        timerMovementBase = timeBetweenMovement;
        timerAttackBase = timeBetweenAttacks;
        timeBetweenAttacks = 0;
        timeBetweenMovement = 0;
        sanityBase = sanity;

        // ATTACKERS
        for (int i = 0; i < attacks.Count; i++)
        {
            _typeAttacks.Add(Instantiate(attacks[i].gameObject, transform.position, Quaternion.identity, transform).GetComponent<AttackBoss>());
        }

        StartCoroutine("Presentation");
    }
    private void Update()
    {
        if (Pause.state != State.Game || LoadingScreen.inLoading) return;

        if (!canInitial) return;

        #region Sanity
        if (sanity < sanityBase)
        {
            sanity += Time.deltaTime;
        }

        if(sanity <= 0) { StartCoroutine("ApplyEffect"); }
        #endregion

        if (canAttack)
        {
            Debug.Log("Puede atacarte...");
            timeBetweenAttacks -= Time.deltaTime;

            if (timeBetweenAttacks <= 0)
            {
                Attack();
                timeBetweenAttacks = timerAttackBase;
            }
        }

        if (canMove)
        {
            timeBetweenMovement -= Time.deltaTime;

            if(timeBetweenMovement <= 0)
            {
                Movement();
                timeBetweenMovement = timerMovementBase;
            }
        }
    }
    // ---- STOPPERS ---- //
    private IEnumerator Presentation()
    {
        yield return new WaitForSeconds(2f);

        // INICIA LA UI DEL BOSS
        _bossUI.StartUIPerBoss(nameBoss, health, shield);

        indexPhase = 0;
        inMove = false;
        canMove = false;
        inAttack = false;
        canAttack = false;
        canInitial = false;
        canTakeDamage = false;
        Pause.StateChange = State.Pause;
        CameraMovement.CallCamera(transform.position, 2f);

        // anim.SetBool("Presentation", true);

        yield return new WaitForSeconds(3.5f);

        // anim.SetBool("Presentation", false);

        Pause.StateChange = State.Game;
        canTakeDamage = true;
        canInitial = true;
        canAttack = true;
        canMove = true;
    }
    private IEnumerator ChangePhase()
    {
        inMove = false;
        canMove = false;
        inAttack = false;
        canAttack = false;
        canTakeDamage = false;
        indexPhase++;

        // anim.SetBool("ChangePhase", true);

        yield return new WaitForSeconds(2f);

        // anim.SetBool("ChangePhase", false);

        canMove = true;
        canAttack = true;
        canTakeDamage = true;
    }
    private IEnumerator Die()
    {
        canMove = false;
        canAttack = false;
        canTakeDamage = false;
        Pause.StateChange = State.Pause;
        CameraMovement.CallCamera(transform.position, 3f);

        // anim.SetBool("Die", true);

        yield return new WaitForSeconds(3.5f);

        Pause.StateChange = State.Game;
        _room.Advance();

        _bossUI.StartCoroutine("HideUI");

        Destroy(gameObject);
    }
    // ---- CALLERS ---- //
    public void TakeDamage(int dmg)
    {
        if (!canTakeDamage) return;

        if (shield >= dmg) { shield -= dmg; }
        else
        {
            dmg -= shield;
            shield = 0;

            if (health >= dmg) health -= dmg;
            else health = 0;
        }

        sanity -= 1;

        // APLICAR CAMBIOS EN LA UI
        _bossUI.SetStats(health, shield);

        if (health <= limiterPerPhase[indexPhase])
        {
            if (indexPhase < (limiterPerPhase.Count - 1)) StartCoroutine("ChangePhase");
        }

        if (health <= 0) { StartCoroutine("Die"); }
    }
    // ---- REPEATERS ---- //
    private void Movement()
    {
        if (inAttack) return;

        int rnd = Random.Range(0, _typeMovement.Count);

        canMove = false;
        inMove = true;

        _typeMovement[rnd].Move(speed);
    }
    private void Attack()
    {
        if (inMove) { CancelMove(); }

        Debug.Log("Éstá pensando su ataque");

        int rnd = Random.Range((indexPhase > 0 ? countAttacksPerPhase[(indexPhase - 1)] : 0), countAttacksPerPhase[indexPhase]);

        if (DistanceToPlayer(_typeAttacks[rnd].distanceToAttack))
        {

            Debug.Log("El Jefe te atacará");
            _typeAttacks[rnd].bossParent = this;
            _typeAttacks[rnd].StartCoroutine("Attack");

            canAttack = false;
            inAttack = true;
        }
        else
        {
            Debug.LogWarning("No puede atacarte, estás muy lejos");
            Movement();
        }
    }
    private IEnumerator ApplyEffect()
    {
        // SE APLICÓ EL EFECTO DE STUN
        Debug.Log("Se aplicó el STUN");
        sanity = sanityBase * 1.5f;

        inMove = false;
        canMove = false;
        inAttack = false;
        canAttack = false;

        yield return new WaitForSeconds(2f);

        canMove = true;
        canAttack = true;
    }
    // ---- SETTERS && GETTERS ---- //
    private bool DistanceToPlayer(float value)
    {
        if (Vector3.Distance(transform.position, _playerStats.transform.position) <= value) { return true; }
        else { return false; }
    }
    public void CancelMove()
    {
        inMove = false;
        canMove = true;
    }
    public void CancelAttack()
    {
        inAttack = false;
        canAttack = true;
    }
    // ---- TRIGGERS ---- //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Proyectile"))
        {
            int damage = 0;

            if (collision.GetComponent<WeaponSystem>())
            {
                WeaponSystem weapon = collision.GetComponent<WeaponSystem>();
                damage = weapon.damage;

                weapon.FinishAttack();
            }
            else if (collision.GetComponent<Projectile>())
            {
                Projectile pr = collision.GetComponent<Projectile>();
                damage = pr.damage;

                // VERIFICA QUE EL PROYECTIL HAYA SIDO LANZADO POR EL JUGADOR
                if (pr.whoIsBoss != 0) return;

                // DESTRUYE EL PROYECTIL SI ESTE NO PUEDE ATRAVESAR OBJETOS
                if (!pr.canTraverse) Destroy(collision.gameObject);
            }

            TakeDamage(damage);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timeToDetectCollision -= Time.deltaTime;

            if(timeToDetectCollision <= 0)
            {
                Debug.Log("Detect Per Collision");
                _playerStats.TakeDamage(gameObject, damageMultiplier);

                // Aplica daño al PLAYER si colisionó con el BOSS por X segundos
                timeToDetectCollision = detectCollisionBase;
            }
        }
        if (collision.CompareTag("Proyectile"))
        {
            if (collision.GetComponent<WeaponSystem>())
            {
                WeaponSystem weapon = collision.GetComponent<WeaponSystem>();
                int dmg = weapon.damage;

                weapon.FinishAttack();

                TakeDamage(dmg);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) timeToDetectCollision = detectCollisionBase;
    }
}