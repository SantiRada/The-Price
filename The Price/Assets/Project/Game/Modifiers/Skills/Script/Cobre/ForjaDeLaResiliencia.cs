using UnityEngine;

public class ForjaDeLaResiliencia : SkillManager {

    public override void Attack()
    {
        Debug.Log("Ataca con " + LanguageManager.GetValue("skill", skillName));
    }
    public override void Passive()
    {
        Debug.Log("Pasiva de " + LanguageManager.GetValue("skill", skillName));
    }
}
