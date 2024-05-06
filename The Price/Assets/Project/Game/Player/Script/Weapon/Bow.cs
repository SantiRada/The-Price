using UnityEngine;

public class Bow : WeaponSystem {

    [Header("Data Weapon for Stats")]
    [SerializeField] private uint _health;
    [SerializeField] private uint _energy;
    [SerializeField] private uint _speed;
    [SerializeField] private uint _damage;
    [SerializeField] private uint _damageSkills;
    [SerializeField] private uint _critical;

    [Header("Data Combo")]
    [SerializeField] private uint _countMaxAttack = 3;
    [SerializeField] private float _range = 3;

    [Header("Data Bow")]
    [SerializeField] private GameObject _projectileObject;
    [SerializeField] private float _timeToSecondArrow;

    [Header("Calls")]
    [HideInInspector] public PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
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
        CreateArrow();

        if (countAttack > (_countMaxAttack - 1)) Invoke("CreateArrow", _timeToSecondArrow);
    }
    public override void Combo()
    {
        if (countAttack >= _countMaxAttack)
        {
            countAttack = 0;
        }
    }
    private void CreateArrow()
    {
        Vector2 dir = Vector2.zero;

        dir = new Vector2(transform.position.x - playerStats.transform.position.x, transform.position.y - playerStats.transform.position.y);

        GameObject pry = Instantiate(_projectileObject, transform.position, transform.rotation);
        pry.gameObject.GetComponent<Projectile>().LaunchProjectile(dir, _range, playerStats.Damage);
    }
}
