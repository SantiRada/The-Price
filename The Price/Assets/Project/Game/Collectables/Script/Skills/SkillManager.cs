using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

        values[0] = LanguageManager.GetValue(_name);
        values[1] = LanguageManager.GetValue(_description);
        values[2] = LanguageManager.GetValue(_featuredUsed);
        values[3] = LanguageManager.GetValue(86) + LanguageManager.GetValue(valueType);
        values[4] = _countForLoad.ToString();

        if (_numberOfLoads != 0) values[5] = LanguageManager.GetValue(85) + _numberOfLoads.ToString();
        else values[5] = "";

        values[6] = _fragmentsYesOrNo.ToString();
        values[7] = _countFragments.ToString();
        values[8] = _damage.ToString();
        values[9] = LanguageManager.GetValue(_infoExtra);

        return values;
    }
    // ------------------------------- //
    public abstract void Attack();
    public abstract void Passive();
    // ------------------------------- //
}
