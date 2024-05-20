using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCollectable : CollectableInScene {

    public GameObject skill; // Es un GameObject de un Prefab de SkillManager

    [Header("Data UI")]
    private TextMeshProUGUI _name;
    private TextMeshProUGUI _description;
    private TextMeshProUGUI _damage;
    private TextMeshProUGUI _loaders;
    private GameObject _sectionDamage;
    private GameObject _sectionLoaders;

    public override void InitialValues()
    {
        TextMeshProUGUI[] _allText = _windowAppear.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in _allText)
        {
            if (text.name.Contains("Title")) _name = text;
            if (text.name.Contains("Description")) _description = text;
            if (text.name.Contains("Loaders")) _loaders = text;
            if (text.name.Contains("Damage")) _damage = text;
        }

        _sectionLoaders = _loaders.GetComponentInParent<Image>().gameObject;
        _sectionDamage = _damage.GetComponentInParent<Image>().gameObject;
    }
    public override void LoadData()
    {
        if (skill == null) return;

        List<string> values = new List<string>();
        values = skill.GetComponent<SkillManager>().GetValuesUI();

        _name.text = values[0];
        _description.text = values[1];
        _damage.text = values[8];
        _loaders.text = values[5];

        if (values[8] != "0") _sectionDamage.SetActive(true);
        else _sectionDamage.SetActive(false);
        
        if (values[5] != "") _sectionLoaders.SetActive(true);
        else _sectionLoaders.SetActive(false);
    }
    public override void Select(GameObject obj)
    {
        GameObject skillData = Instantiate(skill, obj.transform.position, Quaternion.identity, obj.transform);
        obj.GetComponent<ActionForControlPlayer>().SetSkill(skillData, true);
    }
}
