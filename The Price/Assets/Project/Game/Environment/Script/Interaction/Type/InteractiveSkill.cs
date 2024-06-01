using UnityEngine;

public class InteractiveSkill : InteractiveObject {

    public SkillManager _skill;

    [Header("Private Content")]
    private SkillPlacement _placement;
    private PlayerMovement _player;

    private void Start()
    {
        _placement = FindAnyObjectByType<SkillPlacement>();
        _player = FindAnyObjectByType<PlayerMovement>();
        
        ChangeSkill(_placement.RandomPool());
    }
    public void ChangeSkill(SkillManager sk)
    {
        _skill = sk;

        nameContent = _skill._skillName;
        descContent = _skill._descName;
    }
    // --------------------------- //
    private void Update()
    {
        if (inSelect)
        {
            // CAMBIAR EN LA POSICIÓN DE TRIANGULO
            if (Input.GetButtonDown("Fire4"))
            {
                ComprobationForPositionSkill(0);
            }

            // CAMBIAR EN LA POSICIÓN DE CÍRCULO
            if (Input.GetButtonDown("Fire2"))
            {
                ComprobationForPositionSkill(1);
            }
        }
    }
    private void ComprobationForPositionSkill(int pos)
    {
        if (_player.skills.Count > 1) { _player.skills[pos] = _skill; }
        else if (_player.skills.Count == 1)
        {
            if (pos == 0) _player.skills[0] = _skill;
            else _player.skills.Add(_skill);
        }
        else
        {
            if(pos == 0) _player.skills.Add(_skill);

            _player.skills.Add(null);

            if(pos == 1) _player.skills.Add(_skill);
        }

        inSelect = false;
        Destroy(gameObject);
    }
    public override void TakeAttack() { Debug.Log("No ocurre nada ante el golpe"); }
    public override void Select() { Debug.Log("No pasa nada al seleccionarlo"); }
}
