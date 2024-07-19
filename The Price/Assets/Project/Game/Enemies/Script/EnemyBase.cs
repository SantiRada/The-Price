using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    [Range(0, 1), Tooltip("Tiempo en que no detecta daño después de haberlo resibido (intangibilidad)")] public float delayToDetectDamage;
    [Tooltip("Tiempo que tenes que colisionar para que te aplique daño.")] public float timeToDetectCollision;
    private float detectCollisionBase;

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
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _room = FindAnyObjectByType<Room>();

        InitialValues();
    }
    private void InitialValues()
    {
        sanityBase = sanity;
        detectCollisionBase = timeToDetectCollision;
        health = healthMax;
        shield = shieldMax;

        CancelEnemy(false);
    }
    private void LateUpdate()
    {
        if (Pause.state != State.Game || LoadingScreen.inLoading) return;

        #region Sanity
        if (sanity < sanityBase) { sanity += Time.deltaTime; }

        if (sanity <= 0) { StartCoroutine("ApplyEffect"); }
        #endregion
    }
    private IEnumerator ApplyEffect()
    {
        // SE APLICÓ EL EFECTO DE STUN
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
        st.CreateState(state, numberOfLoads);
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
        if (canTakeDamage)
        {
            canTakeDamage = false;
            if (shield >= dmg) { shield -= dmg; }
            else
            {
                dmg -= shield;
                shield = 0;

                if (health >= dmg) health -= dmg;
                else health = 0;
            }

            sanity -= 1;
            _playerStats.SetCountDamage(dmg);

            SpecificTakeDamage(dmg);

            yield return new WaitForSeconds(delayToDetectDamage);

            canTakeDamage = true;

            if (health <= 0)
            {
                if (canReleaseSouls) ManagerGold.CreateSouls((transform.position + new Vector3(0.5f,0.5f, 0)), countSouls);
                if (canReleaseGold) ManagerGold.CreateGold((transform.position + Vector3.one), countGold);

                StartCoroutine("Die");
            }
        }
    }
    public void Attack(int index = -1)
    {
        if (inAttack) return;
        if (inMove) { StartCoroutine("CancelMove"); }

        SpecificAttack(index);

        canAttack = false;
        inAttack = true;
    }
    // ---- TRIGGERS ---- //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Proyectile"))
        {
            int dmg = 0;

            if (collision.GetComponent<WeaponSystem>())
            {
                WeaponSystem weapon = collision.GetComponent<WeaponSystem>();
                dmg = weapon.damage;

                weapon.FinishAttack();
            }
            else if (collision.GetComponent<Projectile>())
            {
                Projectile pr = collision.GetComponent<Projectile>();
                dmg = pr.damage;

                // VERIFICA QUE EL PROYECTIL HAYA SIDO LANZADO POR EL JUGADOR
                if (pr.whoIsBoss != 0) return;

                // DESTRUYE EL PROYECTIL SI ESTE NO PUEDE ATRAVESAR OBJETOS
                if (!pr.canTraverse) Destroy(collision.gameObject);
            }

            StartCoroutine(TakeDamage(dmg));

            _playerStats.ApplyDamage(dmg);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timeToDetectCollision -= Time.deltaTime;

            if (timeToDetectCollision <= 0)
            {
                _playerStats.TakeDamage(gameObject, damage);

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

                StartCoroutine(TakeDamage(dmg));

                _playerStats.ApplyDamage(dmg);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) timeToDetectCollision = detectCollisionBase;
    }
    // ---- GETTERS ---- //
    public bool CanAttack { set { canAttack = value; } get { return canAttack; } }
    public bool CanMove { set { canMove = value; } get { return canMove; } }
    public Room CurrentRoom { set { _room = value; } get { return _room; } }
    public bool InAttack { get { return inAttack; } }
    public bool InMove { get { return inMove; } }
}
