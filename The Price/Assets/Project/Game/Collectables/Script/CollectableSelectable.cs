using System.Collections.Generic;
using UnityEngine;

public enum TypeElement { Skills, Aptitudes, Objects }
public class CollectableSelectable : MonoBehaviour {

    [Header("Data Pool")]
    [SerializeField] private TypeElement _elements;
    [SerializeField] private List<GameObject> _poolBase = new List<GameObject>();

    [Header("Specific Pools")]
    private List<SkillManager> _poolSkill = new List<SkillManager>();

    [Header("Private Data")]
    private SelectorUI _selector;

    private void Awake()
    {
        _selector = GetComponent<SelectorUI>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        for(int i = 0; i < _poolBase.Count; i++)
        {
            if (_elements == TypeElement.Skills) _poolSkill.Add(_poolBase[i].GetComponent<SkillManager>());
        }
    }
    public void RandomValues(int count, GameObject obj)
    {
        _selector.Invoke("WaitForMove", 0.25f);

        PauseMenu.inPause = true;

        for (int i = 0; i < count; i++)
        {
            int position = Random.Range(0, _poolSkill.Count);

            ShowInUI(position, i, obj);

            _poolBase.RemoveAt(position);
            _poolSkill.RemoveAt(position);
        }
    }
    private void ShowInUI(int index, int pos, GameObject obj)
    {
        switch (_elements)
        {
            case TypeElement.Skills:

                List<string> values = _poolSkill[index].GetValuesUI();
                _selector.ShowInUI(_poolBase[index], values, pos, obj);
                break;
        }
    }
}