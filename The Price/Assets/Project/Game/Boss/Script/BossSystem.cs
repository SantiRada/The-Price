using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TypeBoss { minBoss, Boss, maxBoss }
public class BossSystem : MonoBehaviour {

    [Header("Manager Content")]
    public int countPhase;
    [Tooltip("Este BOSS cambia de fase por cantidad de vida (o por tiempo).")] public bool changePhasePerLife;
    [Tooltip("Cantidad de vida o tiempo para cambiar de fase.")] public List<int> limiterPerPhase = new List<int>();
    [Tooltip("Cantidad de ataques que hace en cada fase.")] public List<int> countAttacksPerPhase = new List<int>();
    [Tooltip("Lista total de Ataques.")] public List<AttackBoss> attacks = new List<AttackBoss>();
    private int indexPhase = 0;

    [Header("General Stats")]
    public int nameBoss;
    [Tooltip("Unicamente sirve para el JSON de guardado de muertes.")] public TypeBoss typeBoss;
    public int health;
    public int shield;
    public int damageMultiplier;
    public float speed;
    public float sanity;
    private float sanityBase;

    [Header("Collision")]
    [Tooltip("Tiempo que tenes que colisionar para que te aplique daño.")] public float timeToDetectCollision;
    private float detectCollisionBase;

    [Header("Move & Attack")]
    [Range(0, 3)] public float delayBetweenMovement;
    [Range(0, 5)] public float delayBetweenAttack;
    [Tooltip("Si lo es, tiene más probabilidad de atacar que de moverse por el mapa.")] public bool isAggressive;
    [Tooltip("Si es TRUE, en fases posteriores incluye todos los ataques de fases previas.")] public bool includeAllAttacks;

    [Header("Private Content for Move & Attack")]
    private bool canTakeDamage;
    public bool inMove, canMove;
    public bool inAttack, canAttack;
    private float maxDistance, minDistance;
    private int indexMin;

    [Header("Private Content")]
    private PlayerStats _playerStats;
    private BossUI _bossUI;
    private Room _room;

    [Header("List")]
    private List<TypeMovement> _typeMovement = new List<TypeMovement>();
    private List<AttackBoss> _typeAttacks = new List<AttackBoss>();

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
        sanityBase = sanity;

        // ATTACKERS
        for (int i = 0; i < attacks.Count; i++)
        {
            _typeAttacks.Add(Instantiate(attacks[i].gameObject, transform.position, Quaternion.identity, transform).GetComponent<AttackBoss>());
        }
        ComprobateDistanceToPlayer();

        StartCoroutine("Presentation");
    }
    private void Update()
    {
        if (Pause.state != State.Game || LoadingScreen.inLoading) return;

        #region Sanity
        if (sanity < sanityBase)
        {
            sanity += Time.deltaTime;
        }

        if(sanity <= 0) { StartCoroutine("ApplyEffect"); }
        #endregion

        if (canMove)
        {
            if (!canAttack)
            {
                Movement();
                return;
            }

            if(Vector3.Distance(transform.position, _playerStats.transform.position) > maxDistance) { Movement(); }
            else if(Vector3.Distance(transform.position, _playerStats.transform.position) < minDistance) { Attack(indexMin); }
            else
            {
                int rnd = Random.Range(0, 100);

                if (isAggressive)
                {
                    if(rnd < 70) { Attack(); }
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
        inMove = false;
        canMove = false;
        inAttack = false;
        canAttack = false;
        canTakeDamage = false;
        if((countAttacksPerPhase.Count - 1) > indexPhase) indexPhase++;

        // anim.SetBool("ChangePhase", true);

        yield return new WaitForSeconds(2f);

        // anim.SetBool("ChangePhase", false);

        canMove = true;
        canAttack = true;
        canTakeDamage = true;
    }
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
        canTakeDamage = false;
        Pause.StateChange = State.Pause;
        CameraMovement.CallCamera(transform.position, 2f);

        // anim.SetBool("Presentation", true);

        yield return new WaitForSeconds(3.5f);

        // anim.SetBool("Presentation", false);

        Pause.StateChange = State.Game;
        canTakeDamage = true;
        canAttack = true;
        canMove = true;
    }
    private IEnumerator Die()
    {
        canMove = false;
        canAttack = false;
        canTakeDamage = false;
        Pause.StateChange = State.Pause;

        StartCoroutine("CancelMove");
        StartCoroutine("CancelAttack");

        yield return new WaitForSeconds(0.5f);

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
            if (indexPhase < countPhase) StartCoroutine("ChangePhase");
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
    private void Attack(int index = -1)
    {
        if (inMove) { StartCoroutine("CancelMove"); }

        #region CalculateTypeAttack
        int minValue;

        if (includeAllAttacks) minValue = 0;
        else minValue = (indexPhase > 0 ? countAttacksPerPhase[(indexPhase - 1)] : 0);

        int rnd = Random.Range(minValue, countAttacksPerPhase[indexPhase]);

        if (index != -1) rnd = index;
        #endregion

        _typeAttacks[rnd].bossParent = this;
        _typeAttacks[rnd].StartCoroutine("Attack");

        canAttack = false;
        inAttack = true;
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
    // ---- FUNCION INTEGRA ---- //
    private void ComprobateDistanceToPlayer()
    {
        minDistance = 1000;
        maxDistance = 0;

        for (int i = 0; i < _typeAttacks.Count; i++)
        {
            // GUARDA EL ATAQUE DE MÍNIMA DISTANCIA
            if (_typeAttacks[i].distanceToAttack < minDistance)
            {
                minDistance = _typeAttacks[i].distanceToAttack;
                indexMin = i;
            }

            // GUARDA EL ATAQUE DE MÁXIMA DISTANCIA
            if (_typeAttacks[i].distanceToAttack > maxDistance) { maxDistance = _typeAttacks[i].distanceToAttack; }
        }
    }
    // ---- SETTERS && GETTERS ---- //
    public IEnumerator CancelMove()
    {
        for(int i = 0; i< _typeMovement.Count; i++) { _typeMovement[i].CancelMove(); }

        inMove = false;

        yield return new WaitForSeconds(delayBetweenMovement);

        if(health > 0) canMove = true;
    }
    public IEnumerator CancelAttack()
    {
        inAttack = false;

        yield return new WaitForSeconds(delayBetweenAttack);

        if (health > 0) canAttack = true;
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