using UnityEngine;

public class PlayerStats : MonoBehaviour {

    [Header("Stats")]
    private string _control { get; set; }
    [SerializeField] private uint _healthMax;
    [SerializeField] private uint _energyMax;
    [Space]
    private uint _health;
    private uint _energy;
    [Space]
    private uint _speed;
    private uint _damage;
    private uint _damageSkills;
    private uint _critical;

    [Header("Calls")]
    private Animator anim;
    private DataPlayerHUD _hud;
    [HideInInspector] public DataPlayerStats _stats;
    private WeaponSystem _weapon;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _weapon = GetComponentInChildren<WeaponSystem>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _control.Contains("Mouse")) _weapon.CallAttack();
    }
    private void InitialValues()
    {
        _health = _healthMax;
        _energy = _energyMax;
        // ----------------------- //
        _stats._textHealth.text = GetHealthUI();
        _stats._textEnergy.text = GetEnergyUI();
        _stats._textSpeed.text = _speed.ToString();
        _stats._textDamage.text = _damage.ToString();
        _stats._textDamageSkills.text = _damageSkills.ToString();
        _stats._textCritical.text = _critical.ToString();
        // ----------------------- //
        _hud.healthBar.fillAmount = 1;
        _hud.energyBar.fillAmount = 1;

        _stats.gameObject.SetActive(false);
    }
    public void SetUI(GameObject obj, GameObject stats, string data)
    {
        _hud = obj.GetComponent<DataPlayerHUD>();
        _stats = stats.GetComponent<DataPlayerStats>();
        _control = data;

        InitialValues();
    }
    private string GetHealthUI()
    {
        return _health.ToString() + "/" + _healthMax.ToString();
    }
    private string GetEnergyUI()
    {
        return _energy.ToString() + "/" + _energyMax.ToString();
    }
    // ----------------------- //
    public string Control
    {
        get { return _control; }
        set { _control = value; }
    }
    public uint Health
    {
        get { return _health; }
        set {
            _health = value;
            _hud.healthBar.fillAmount = _health / 1;
        }
    }
    public uint Energy
    {
        get { return _energy; }
        set {
            _energy = value;
            _hud.energyBar.fillAmount = _energy / 1;
        }
    }
    public uint Speed
    {
        get { return _speed; }
        set {
            _speed = value;
            _stats._textSpeed.text = _speed.ToString();
        }
    }
    public uint Damage
    {
        get { return _damage; }
        set {
            _damage = value;
            _stats._textDamage.text = _damage.ToString();
        }
    }
    public uint DamageSkills
    {
        get { return _damageSkills; }
        set {
            _damageSkills = value;
            _stats._textDamageSkills.text = _damageSkills.ToString();
        }
    }
    public uint CriticalChance
    {
        get { return _critical; }
        set {
            _critical = value;
            _stats._textCritical.text = _critical.ToString();
        }
    }
    // ----------------------- //
    public void SetAnimatorInt(string type, int value)
    {
        anim.SetInteger(type, value);
    }
    public void SetAnimatorBool(string type, bool value)
    {
        anim.SetBool(type, value);
    }
    public void SetAnimatorFloat(string type, float value)
    {
        anim.SetFloat(type, value);
    }
    // ----------------------- //x
    public int GetAnimatorInt(string type)
    {
        return anim.GetInteger(type);
    }
    public bool GetAnimatorBool(string type)
    {
        return anim.GetBool(type);
    }
    public float GetAnimatorFloat(string type)
    {
        return anim.GetFloat(type);
    }
    // ----------------------- //
    public void TakeDamage(uint damage)
    {
        if(_health <= damage)
        {
            if(_health > (damage / 2)) _health = 1;
            else _health = 0;
        }
        else
        {
            _health -= damage;
        }

        _hud.healthBar.fillAmount -= (float)damage / (float)_healthMax;
    }
    public void DropEnergy(uint value)
    {
        if (value > _energy) return;

        _energy -= value;

        float valueUI = (float)value / (float)_energyMax;

        _hud.energyBar.fillAmount -= valueUI;
    }
    // ----------------------- //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            TakeDamage(10);
        }
    }
}