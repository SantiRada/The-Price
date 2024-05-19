using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeElement { Skills, Aptitudes, Objects }
public class CollectableSelectable : MonoBehaviour {

    [Header("Data Pool")]
    [SerializeField] private TypeElement _elements;
    [SerializeField] private GameObject[] _pool;

    [Header("Data UI")]
    [SerializeField] private TextMeshProUGUI[] _name;
    [SerializeField] private TextMeshProUGUI[] _description;

    [Header("Data UI: Skills")]
    [SerializeField] private TextMeshProUGUI[] _type;
    [SerializeField] private GameObject[] _sectionLoaders;
    [SerializeField] private TextMeshProUGUI[] _loaders;
    [SerializeField] private GameObject[] _sectionFragments;
    [SerializeField] private TextMeshProUGUI[] _fragments;
    [SerializeField] private GameObject[] _sectionDamage;
    [SerializeField] private TextMeshProUGUI[] _damage;

    [Header("Data Extra")]
    [SerializeField] private List<int> _featuredUsed = new List<int>();
    [SerializeField] private List<int> _infoExtra = new List<int>();

    [Header("Private Data")]
    List<SkillManager> _skills = new List<SkillManager>();
    private LanguageManager _language;
    private SelectorUI _selector;

    private void Awake()
    {
        _language = FindAnyObjectByType<LanguageManager>();
        _selector = GetComponent<SelectorUI>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        if (_elements == TypeElement.Skills)
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                _skills.Add(_pool[i].GetComponent<SkillManager>());
            }
        }
    }
    public void RandomValues(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int position = Random.Range(0, _skills.Count);

            ShowInUI(i, position);

            // REMOVER EL ELEMENTO SELECCIONADO DE LA POOL
            _skills.RemoveAt(position);
        }

        _selector._featuredPosition = _featuredUsed;
        _selector._infoPosition = _infoExtra;
    }
    private void ShowInUI(int index, int pos)
    {
        switch (_elements)
        {
            case TypeElement.Skills:

                List<int> values = new List<int>();
                values.AddRange(_skills[pos].GetValuesUI());

                _name[index].text = _language.GetValue(values[0]);
                _description[index].text = _language.GetValue(values[1]);
                _type[index].text = _language.GetValue(86) + _language.GetValue(values[3]);
                _loaders[index].text = _language.GetValue(85) + values[5];
                if (values[6] == 1)
                {
                    _sectionFragments[index].SetActive(true);
                    _fragments[index].text = values[7].ToString();
                }
                if (values[8] != 0)
                {
                    _sectionDamage[index].SetActive(true);
                    _damage[index].text = values[8].ToString();
                }

                _featuredUsed.Add(values[2]);
                _infoExtra.Add(values[9]);
                break;
        }
    }
}
/*
        values[0] = _name.ToString();
        values[1] = _description.ToString();
        values[2] = _featuredUsed.ToString();
        values[3] = _type.ToString();
        values[4] = _countForLoad.ToString();
        values[5] = _numberOfLoads.ToString();
        values[6] = _requiredFragments.ToString();
        values[7] = _countFragments.ToString();
        values[8] = _damage.ToString();
        values[9] = infoExtra.ToString();
*/