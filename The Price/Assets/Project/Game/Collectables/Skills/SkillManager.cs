using UnityEngine;

public enum TypeSkill { Cobre, Titanio, Tungsteno, Carbono }
public abstract class SkillManager : MonoBehaviour {

    [Header("Data Skill")]
    [SerializeField] protected int _name;
    [SerializeField] protected int _description;
    [SerializeField] protected int _featuredUsed;
    [SerializeField] protected TypeSkill _type;
    [SerializeField] protected int _countForLoad;
    [SerializeField] protected int _numberOfLoads;

    [Header("Data Fragments")]
    [SerializeField] protected bool _requiredFragments;
    [SerializeField] protected int _countFragments;

    [Header("Data Values")]
    [SerializeField] protected GameObject _createdElement;
    [SerializeField] protected int _damage;

    private void Update()
    {
        if (LoadingScreen.InLoading) return;

        Passive();
    }
    public string[] GetValuesUI()
    {
        string[] values = new string[9];

        values[0] = _name.ToString();
        values[1] = _description.ToString();
        values[2] = _featuredUsed.ToString();
        values[3] = _type.ToString();
        values[4] = _countForLoad.ToString();
        values[5] = _numberOfLoads.ToString();
        values[6] = _requiredFragments.ToString();
        values[7] = _countFragments.ToString();
        values[8] = _damage.ToString();

        return values;
    }
    // ------------------------------- //
    public abstract void Attack();
    public abstract void Passive();
    // ------------------------------- //
}
