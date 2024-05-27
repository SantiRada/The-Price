using UnityEngine;

public class NeuroestimulacionMetalica : SkillManager {

    public override void Attack()
    {
        Debug.Log("Ataca con " + LanguageManager.GetValue("skill", _skillName));
    }
    public override void Passive()
    {
        Debug.Log("Pasiva de " + LanguageManager.GetValue("skill", _skillName));
    }
}
