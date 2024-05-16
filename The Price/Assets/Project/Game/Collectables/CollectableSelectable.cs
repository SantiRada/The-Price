using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TypeElement { Skills, Aptitudes, Objects }
public class CollectableSelectable : MonoBehaviour {

    [Header("Data Pool")]
    [SerializeField] private TypeElement _elements;
    [SerializeField] private GameObject[] _pool;

    [Header("Data Player")]
    [SerializeField] private int _indexPlayer = 0;

    [Header("Data UI")]
    [SerializeField] private GameObject _windowForSelect;
    [SerializeField] private TextMeshProUGUI[] _name;
    [SerializeField] private TextMeshProUGUI[] _description;

    [Header("Data UI: Skills")]
    [SerializeField] private TextMeshProUGUI[] _featuredUsed;
    [SerializeField] private TextMeshProUGUI[] _type;
    [SerializeField] private TextMeshProUGUI[] _loaders;
    [SerializeField] private TextMeshProUGUI[] _fragments;
    [SerializeField] private TextMeshProUGUI[] _damage;

    [Header("Private Data")]
    List<SkillManager> _skills = new List<SkillManager>();
    private LanguageManager _language;

    private void Awake()
    {
        _language = FindAnyObjectByType<LanguageManager>();
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
    public void LoadScreen()
    {
        _windowForSelect.SetActive(true);

        RandomValues(3);
    }
    public void RandomValues(int count)
    {
        for(int i = 0; i < count; i++)
        {
            int position = Random.Range(0, _skills.Count);

            ShowInUI(i, position);

            // REMOVER EL ELEMENTO SELECCIONADO DE LA POOL
            _skills.RemoveAt(position);
        }
    }
    private void ShowInUI(int index, int pos)
    {
        switch (_elements)
        {
            case TypeElement.Skills:

                string[] values = _skills[pos].GetValuesUI();

                _name[index].text = _language.GetValue(int.Parse(values[0]));
                _description[index].text = _language.GetValue(int.Parse(values[1]));
                _featuredUsed[index].text = _language.GetValue(int.Parse(values[2]));
                _type[index].text = _language.GetValue(int.Parse(values[3]));
                _loaders[index].text = "c" + _language.GetValue(int.Parse(values[4])) + " // n" + _language.GetValue(int.Parse(values[5]));

                if (values[6] == "true") _fragments[index].text = "f" + _language.GetValue(int.Parse(values[7]));
                else _fragments[index].text = "";

                if (values[8] != "0") _damage[index].text = _language.GetValue(int.Parse(values[8]));
                else _damage[index].text = "";

                break;
        }
    }
}