using UnityEngine;
using static ActionForControlPlayer;

public class InteractiveSkill : Interactive {

    public SkillManager skill;

    [Header("Private Content")]
    private SkillPlacement _placement;
    
    private void Start() { _placement = FindAnyObjectByType<SkillPlacement>(); }
    // --------------------------- //
    private void Update()
    {
        if (inTrigger && !isShop)
        {
            // CAMBIAR EN LA POSICIÓN DE TRIANGULO
            if (PlayerActionStates.IsSkillOne) ComprobationForPositionSkill(0);

            // CAMBIAR EN LA POSICIÓN DE CÍRCULO
            if (PlayerActionStates.IsSkillTwo) ComprobationForPositionSkill(1);
        }
    }
    private void ComprobationForPositionSkill(int pos)
    {
        if (_player.skills.Count > 1) { _player.skills[pos] = skill; }
        else if (_player.skills.Count == 1)
        {
            if (pos == 0) _player.skills[0] = skill;
            else _player.skills.Add(skill);
        }
        else
        {
            if(pos == 0) _player.skills.Add(skill);

            _player.skills.Add(null);

            if(pos == 1) _player.skills.Add(skill);
        }

        inSelect = false;

        CloseWindow();

        Destroy(gameObject);
    }
    public override void RandomPool()
    {
        skill = _placement.RandomPool();

        nameContent = skill.skillName.ToString();
        descContent = skill.descName.ToString();
    }
    public override void Select()
    {
        if (isShop)
        {
            Buy();
            return;
        }

        Debug.Log("No pasa nada al seleccionar una Skill...");
    }
}
