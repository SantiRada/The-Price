using UnityEngine;

public class UselessSkillPerType : MonoBehaviour {

    public TypeEnemyAttack typeUseless;
    private Room _roomInScene;
    private SkillManager _skill;

    private void Start()
    {
        _roomInScene = FindAnyObjectByType<Room>();
        _skill = GetComponent<SkillManager>();

        _roomInScene.SetUselessEnemies(typeUseless, false);

        _skill.destroyElement += () => _roomInScene.SetUselessEnemies(typeUseless, true);
    }
}
