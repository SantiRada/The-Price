using System.Collections;
using UnityEngine;

public enum TypeEnemyAttack { Base, Energy, Fire, Cold, Fortify }
public abstract class EnemyManager : EnemyBase {

    [Header("UI Content")]
    public GameObject uiObject;
    public Vector2 offsetPositionUI;
    private Canvas _worldPosition;
    private EnemyUI _enemyUI;

    [Header("Initial Values")]
    public int _weight;
    [Range(0, 100)] public int probabilityOfAppearing;
    [Range(0f, 5f)] public float initialDelay;

    [Header("Attack Enemy")]
    public bool distanceAttack;
    public TypeEnemyAttack typeAttack;
    [Tooltip("Distancia necesaria para atacar")] public float rangeAttack;

    [Header("Private Data")]
    protected Animator _anim;
    protected Rigidbody2D _rb2d;
    protected SpriteRenderer _spr;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        _worldPosition = FindAnyObjectByType<InteractiveManager>().GetComponent<Canvas>();


        // CREAR UI PARA CADA ENEMIGO
        _enemyUI = Instantiate(uiObject, ((Vector2)transform.position + offsetPositionUI), Quaternion.identity, _worldPosition.transform).GetComponent<EnemyUI>();
        _enemyUI.SetInitialValues(gameObject, offsetPositionUI);

        StartCoroutine("DelayToMovement");
    }
    private void Update()
    {
        if (canMove)
        {
            if (_playerStats.transform.position.x > transform.position.x) _spr.flipX = true;
            else _spr.flipX = false;

            if(_playerStats.transform.position.y > transform.position.y) _anim.SetBool("Direction", false);
            else _anim.SetBool("Direction", true);
        }

        if (Vector3.Distance(_playerStats.transform.position, transform.position) <= rangeAttack) { if (canAttack) Attack(); }
    }
    // ---- ABSTRACT ---- //
    public abstract override void SpecificAttack(int index);
    // ---- OVERRIDE ---- //
    public override IEnumerator Die()
    {
        _room?.SetLivingEnemies(this);

        Destroy(_enemyUI.gameObject);

        Destroy(gameObject, 1);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
    public override void SpecificTakeDamage(int dmg)
    {
        _enemyUI.SetHealthbar(healthMax, health, shieldMax, shield);

        FloatTextManager.CreateText(transform.position, TypeColor.Damage, "-" + dmg.ToString());
    }
    public override void SpecificMove() { Debug.Log("No es específico"); }
    public override void SpecificState(TypeState state, int numberOfLoads)
    {
        if (state == TypeState.Stun && numberOfLoads == 2)
        {
            if (Weight > 1) numberOfLoads = 1;
        }
    }
    // ---- FUNCION INTEGRA ---- //
    public IEnumerator DelayToMovement()
    {
        CancelEnemy(false);
        yield return new WaitForSeconds(initialDelay);
        CancelEnemy(true);
    }
    // ---- SETTERS && GETTERS ---- //
    public int Weight { get { return _weight; } }
}