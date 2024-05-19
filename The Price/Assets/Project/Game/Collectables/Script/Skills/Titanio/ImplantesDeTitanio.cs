using UnityEngine;

public class ImplantesDeTitanio : SkillManager {

    public override void Attack()
    {
        Debug.Log("Ataqué");
    }
    public override void Passive()
    {
        Debug.Log("Estoy en Habilidades Pasivas");
    }

}
