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
    [SerializeField] protected int _infoExtra;

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
    public int[] GetValuesUI()
    {
        int[] values = new int[10];

        int valueType = 82;
        if (_type == TypeSkill.Cobre) valueType = 79;
        else if (_type == TypeSkill.Titanio) valueType = 80;
        else if (_type == TypeSkill.Carbono) valueType = 81;

        int _fragmentsYesOrNo = _requiredFragments ? 1 : 0;

        values[0] = _name;
        values[1] = _description;
        values[2] = _featuredUsed;
        values[3] = valueType;
        values[4] = _countForLoad;
        values[5] = _numberOfLoads;
        values[6] = _fragmentsYesOrNo;
        values[7] = _countFragments;
        values[8] = _damage;
        values[9] = _infoExtra;

        return values;
    }
    // ------------------------------- //
    public abstract void Attack();
    public abstract void Passive();
    // ------------------------------- //
}
