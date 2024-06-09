using UnityEngine;

public class PreventDamage : MonoBehaviour {

    public TypeEnemyAttack typePrevent;
    private SkillManager _skillManager;

    private void Start()
    {
        _skillManager = GetComponent<SkillManager>();


    }
}
