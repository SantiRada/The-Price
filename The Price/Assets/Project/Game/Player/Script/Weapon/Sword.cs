using UnityEngine;

public class Sword : WeaponSystem {

    [Header("Data Weapon for Stats")]
    [SerializeField] private uint _health;
    [SerializeField] private uint _energy;
    [SerializeField] private uint _speed;
    [SerializeField] private uint _damage;
    [SerializeField] private uint _damageSkills;
    [SerializeField] private uint _critical;

    [Header("Data Combo")]
    [SerializeField] private uint _countMaxAttack = 3;
    [SerializeField] private uint _range = 3;

    [Header("Calls")]
    [HideInInspector] public PlayerStats playerStats;
    private CircleCollider2D _col2D;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        _col2D = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _col2D.radius = _range;

        delayDetectBase = delayDetectAttack;

        playerStats.Health = _health;
        playerStats.Energy = _energy;
        playerStats.Speed = _speed;
        playerStats.Damage = _damage;
        playerStats.DamageSkills = _damageSkills;
        playerStats.CriticalChance = _critical;
    }
    public override void Attack()
    {
        Debug.Log("Atacar");
    }
    public override void Combo()
    {
        Debug.Log("Combo");

        if (countAttack >= _countMaxAttack)
        {
            countAttack = 0;
        }
    }
}
