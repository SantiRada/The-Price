using System.Collections.Generic;
using UnityEngine;

public enum TypeElement { Skills, Aptitudes, Objects }
public class CollectableSelectable : MonoBehaviour {

    [Header("Data Pool")]
    [SerializeField] private TypeElement _elements;
    [SerializeField] private List<SkillManager> _poolSkill = new List<SkillManager>();

    [Header("Private Data")]
    private SelectorUI _selector;

    private void Awake()
    {
        _selector = GetComponent<SelectorUI>();
    }
    public void RandomValues(int count)
    {
        _selector.Invoke("WaitForMove", 0.25f);

        for (int i = 0; i < count; i++)
        {
            int position = Random.Range(0, _poolSkill.Count);

            ShowInUI(position, i);

            _poolSkill.RemoveAt(position);
        }
    }
    private void ShowInUI(int index, int pos)
    {
        switch (_elements)
        {
            case TypeElement.Skills:

                List<string> values = _poolSkill[index].GetValuesUI();
                _selector.ShowInUI(values, pos);
                break;
        }
    }
}