using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    [Header("General Values")]
    [SerializeField] private uint _hp;
    [SerializeField] private uint _concentration;
    [SerializeField] private float _speedMove;
    [SerializeField] private float _speedAttack;
    [SerializeField] private uint _damage;

    [Header("Attack Values")]
    [SerializeField] private float _subsequentDamage;
    [SerializeField] private float _criticChance;
    [SerializeField] private float _missChance;

    [Header("Modifiers")]
    [SerializeField] private float _hpStealing;
    [SerializeField] private float _hpRegeneration;
    [SerializeField] private float _concentrationRegeneration;
    [SerializeField] private int _sanity;

    [Header("Content UI")]
    public GameObject statsWindow;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI concentrationText;
    public TextMeshProUGUI speedMoveText;
    public TextMeshProUGUI speedAttackText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI subsequentDamageText;
    public TextMeshProUGUI criticChanceText;
    public TextMeshProUGUI missChanceText;
    public TextMeshProUGUI hpStealingText;
    public TextMeshProUGUI hpRegenerationText;
    public TextMeshProUGUI concentrationRegenerationText;
    public TextMeshProUGUI sanityText;

    private void Start()
    {
        ChangeValue(-1);

        statsWindow.SetActive(false);
    }
    private void ChangeValue(int type)
    {
        if (type == 0 || type == -1) hpText.text = _hp.ToString();
        if (type == 1 || type == -1) concentrationText.text = _concentration.ToString();
        if (type == 2 || type == -1) speedMoveText.text = _speedMove.ToString();
        if (type == 3 || type == -1) speedAttackText.text = _speedAttack.ToString();
        if (type == 4 || type == -1) damageText.text = _damage.ToString();
        if (type == 5 || type == -1) subsequentDamageText.text = _subsequentDamage.ToString() + "%";
        if (type == 6 || type == -1) criticChanceText.text = _criticChance.ToString() + "%";
        if (type == 7 || type == -1) missChanceText.text = _missChance.ToString() + "%";
        if (type == 8 || type == -1) hpStealingText.text = _hpStealing.ToString() + "%";
        if (type == 9 || type == -1) hpRegenerationText.text = _hpRegeneration.ToString() + "%";
        if (type == 10 || type == -1) concentrationRegenerationText.text = _concentrationRegeneration.ToString() + "%";
        if (type == 11 || type == -1) sanityText.text = _sanity.ToString();
    }
    public void ShowStats()
    {
        if(Pause.state == State.Interface)
        {
            Pause.StateChange = State.Game;
            statsWindow.SetActive(false);
        }
        else
        {
            Pause.StateChange = State.Interface;
            statsWindow.SetActive(true);
        }
    }
    // ---- SETTERS ---- //
    public uint TakeDamage { set { _hp -= value; ChangeValue(0); } }
    public uint HarvestConcentration { set { _concentration += value; ChangeValue(1); } }
    // ---- GETTERS ---- //
    public uint HP { get { return _hp; } }
    public uint Concentration { get { return _concentration; } }
    public float SpeedMove { get {  return _speedMove; } }
    public float SpeedAttack { get {  return _speedAttack; } }
    public uint Damage { get { return _damage; } }
    public float SubsequentDamage { get { return _subsequentDamage; } }
    public float CriticChance { get {  return _criticChance; } }
    public float MissChance {  get { return _missChance; } }
    public float StealingHP { get { return _hpStealing; } }
    public float RegenerationHP { get { return _hpRegeneration; } }
    public float RegenerationConcentration { get { return _concentrationRegeneration; } }
    public int Sanity { get { return _sanity; } }
}
