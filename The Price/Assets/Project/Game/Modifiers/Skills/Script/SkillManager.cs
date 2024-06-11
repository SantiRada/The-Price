using System;
using UnityEngine;

public enum TypeShowSkill { abovePlayer, launched, created }
public enum LoadTypeSkill { concentration, damage, kills, receiveDamage }
public enum TypeDestroySkill { time, receiveDamage, finishRoom, collision }
public abstract class SkillManager : MonoBehaviour {

    [Header("Data Skill")]
    public Sprite icon;
    public int skillName;
    public int descName;
    public int featureUsed;
    [Space]
    [Tooltip("Cantidad de LoadType(fuel) necesaria para poder lanzar esta habilidad")] public int amountFuel;
    public int damage;
    [Tooltip("Cantidad de 'balas'(elementos) creados al lanzarse")] public int countCreated;

    [Header("Content Info")]
    public TypeShowSkill typeShow;
    public LoadTypeSkill loadType;
    [SerializeField] private TypeDestroySkill destroyType;
    [SerializeField] private float timeToDestroy;

    [Header("Prevent Damage")]
    [Tooltip("Tildarlo significa que se prevendrá un % de daño por tipo de enemigo (fire, cold, etc)")] public bool preventDamagePerType;
    [Tooltip("0 = base || 1 = energy || 2 = fire || 3 = cold || 4 = fortify")] public int[] countPrevent = new int[5];
    [Tooltip("0 = base || 1 = energy || 2 = fire || 3 = cold || 4 = fortify")] public bool[] reflects = new bool[5];
    [Space]
    [Tooltip("Tildarlo significa que se prevendrá un % de daño por melee o distancia")] public bool preventDistanceDamage;
    [Tooltip("0 = melee || 1 = distance")] public int[] preventDistance = new int[2];
    [Tooltip("0 = melee || 1 = distance")] public bool[] reflectDistance = new bool[2];

    [Header("Data State")]
    public bool hasState;
    public TypeState state;
    public int countOfLoads;
    public bool hasStatePerAttack;

    [Header("Content")]
    protected PlayerStats _player;
    [HideInInspector] public event Action destroyElement;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerStats>();
    }
    private void Start()
    {
        // APLICAR AFECCIÓN DE UN ESTADO EN LOS ATAQUES DEL PLAYER
        if (hasStatePerAttack) _player.AddStatePerDamage(state, countOfLoads);

        LaunchedSkill();
    }
    private void Update()
    {
        // MANTENER LA SKILL EN LA POSICIÓN DEL PLAYER PARA "abovePlayer"
        if (typeShow == TypeShowSkill.abovePlayer) transform.position = _player.transform.position;

        if (destroyType != TypeDestroySkill.time) return;
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            DestroySkill();
            timeToDestroy = 1000;
        }
    }
    protected void LaunchedSkill()
    {
        // VERIFICAR EL TIPO DE DESTRUCCIÓN DE LA HABILIDAD
        if (destroyType == TypeDestroySkill.finishRoom) RoomManager.finishRoom += DestroySkill;
        else if (destroyType == TypeDestroySkill.receiveDamage) PlayerStats.takeDamage += DestroySkill;

        // VERIFICA SI PREVIENE ALGUN TIPO DE DAÑO Y LO APLICA AL PLAYER
        if (preventDamagePerType) { _player.PreventDamagePerType(countPrevent, reflects); }
        if (preventDistanceDamage) { _player.PreventDamagePerDistance(preventDistance, reflectDistance); }

        // ACTIVAR LA HABILIDAD Y LANZARLA
        TakeEffect();
    }
    protected abstract void TakeEffect();
    protected virtual void DestroySkill()
    {
        // VERIFICA SI PREVIENE ALGUN TIPO DE DAÑO Y LO REMUEVE DEL PLAYER
        if (preventDamagePerType)
        {
            int[] count = { 0, 0, 0, 0, 0 };
            bool[] reflectFalse = { false, false, false, false, false };
            _player.PreventDamagePerType(count, reflectFalse);
        }

        destroyElement?.Invoke();

        // QUITAR AFECCION DE ALGUN ESTADO EN EL ATAQUE DEL PLAYER
        if (hasStatePerAttack) _player.AddStatePerDamage(TypeState.Null, 0);

        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(damage != 0) collision.GetComponent<EnemyManager>().TakeDamage(CalculateDamage());

            if (destroyType == TypeDestroySkill.collision) DestroySkill();
        }
    }
    // ---- FUNCIÓN INTEGRA ---- //
    private int CalculateDamage()
    {
        return (int)(damage * _player.GetterStats(4, false));
    }
}