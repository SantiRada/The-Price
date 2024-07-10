using UnityEngine;

public class EnemyForAttack : EnemyManager {

    public AttackBoss attack;
    private AttackBoss _typeAttack;

    private void OnEnable() { _typeAttack = Instantiate(attack.gameObject, transform.position, Quaternion.identity, transform).GetComponent<AttackBoss>(); }
    public override void SpecificAttack(int index = -1)
    {
        _typeAttack.enemyParent = GetComponent<EnemyBase>();
        _typeAttack.StartCoroutine("Attack");
    }
}
