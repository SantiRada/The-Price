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
    protected bool isActive = false;

    [Header("Content Info")]
    public TypeShowSkill typeShow;
    public LoadTypeSkill loadType;
    [SerializeField] private TypeDestroySkill destroyType;
    [SerializeField] private float timeToDestroy;

    [Header("Prevent Damage")]
    public bool preventDamage;
    public int[] countPrevent = new int[5];

    [Header("Content")]
    protected PlayerStats _player;
    [HideInInspector] public event Action destroyElement;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerStats>();
    }
    private void Start()
    {
        LaunchedSkill();
    }
    private void Update()
    {
        if (destroyType != TypeDestroySkill.time || !isActive) return;

        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0) DestroySkill();
    }
    protected void LaunchedSkill()
    {
        // VERIFICAR EL TIPO DE DESTRUCCIÓN DE LA HABILIDAD
        if (destroyType == TypeDestroySkill.finishRoom) RoomManager.finishRoom += DestroySkill;
        else if (destroyType == TypeDestroySkill.receiveDamage) PlayerStats.takeDamage += DestroySkill;

        // VERIFICA SI PREVIENE ALGUN TIPO DE DAÑO Y LO APLICA AL PLAYER
        if (preventDamage) _player.PreventDamage(countPrevent);

        // ACTIVAR LA HABILIDAD Y LANZARLA
        isActive = true;
        TakeEffect();
    }
    protected abstract void TakeEffect();
    protected virtual void DestroySkill()
    {
        // VERIFICA SI PREVIENE ALGUN TIPO DE DAÑO Y LO REMUEVE DEL PLAYER
        
        if (preventDamage)
        {
            int[] count = { 0, 0, 0, 0, 0 };
            _player.PreventDamage(count);
        }

        destroyElement?.Invoke();

        Destroy(gameObject, 0.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isActive)
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