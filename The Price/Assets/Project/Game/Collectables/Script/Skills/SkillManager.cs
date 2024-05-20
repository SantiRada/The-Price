using System.Collections.Generic;
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
    public List<string> GetValuesUI()
    {
        List<string> values = new List<string>();

        int valueType = 82;
        if (_type == TypeSkill.Cobre) valueType = 79;
        else if (_type == TypeSkill.Titanio) valueType = 80;
        else if (_type == TypeSkill.Carbono) valueType = 81;

        int _fragmentsYesOrNo = _requiredFragments ? 1 : 0;

        values.Add(LanguageManager.GetValue(_name));
        values.Add(LanguageManager.GetValue(_description));
        values.Add(LanguageManager.GetValue(_featuredUsed));
        values.Add(LanguageManager.GetValue(86) + LanguageManager.GetValue(valueType));
        values.Add(_countForLoad.ToString());

        if (_numberOfLoads != 0) values.Add(LanguageManager.GetValue(85) + _numberOfLoads.ToString());
        else values.Add("");

        values.Add(_fragmentsYesOrNo.ToString());
        values.Add(_countFragments.ToString());
        values.Add(_damage.ToString());
        if (_infoExtra != 0) values.Add(LanguageManager.GetValue(_infoExtra));
        else values.Add("");

        return values;
    }
    // ------------------------------- //
    public abstract void Attack();
    public abstract void Passive();
    // ------------------------------- //
}
