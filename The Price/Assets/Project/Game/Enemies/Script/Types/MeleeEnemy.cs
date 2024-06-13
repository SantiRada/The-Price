using UnityEngine;

public class MeleeEnemy : EnemyManager {

    [Header("Melee Content")]
    public GameObject objMelee;
    public float distanceMelee;
    private bool isAttacking = false;

    private void LateUpdate()
    {
        objMelee.transform.position = (Vector2)transform.position + CalculateMeleePosition();

        // MODIFICAR TAG DEL OBJETO = ATAQUE MELEE
        if (isAttacking) { objMelee.tag = "EnemyAttack"; }
        else { objMelee.tag = "Untagged"; }
    }
    public override void Attack() { CanAttack = false; _anim.SetBool("Attack", true); }
    // ---- FUNCIÓN DE EVENTO DE ANIMACIÓN ---- //
    public void InitialAttack() { isAttacking = true; }
    public void FinishAttack() { _anim.SetBool("Attack", false); isAttacking = false; }
    // ---- FUNCIÓN INTEGRA ---- //
    private Vector2 CalculateMeleePosition()
    {
        Vector2 position = -(transform.position - _player.transform.position);
        position.Normalize();

        position *= distanceMelee;

        return position;
    }
}