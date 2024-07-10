using UnityEngine;

public class MeleeEnemy : EnemyManager {

    [Header("Melee Content")]
    public float distanceMelee;
    public float timeBetweeenUpdateLook;
    private DetectorPlayer _objMelee;
    private float timeBetweenBase;

    private void OnEnable()
    {
        _objMelee = GetComponentInChildren<DetectorPlayer>();
        _objMelee.damage = damage;
        timeBetweenBase = timeBetweeenUpdateLook;
    }
    private void LateUpdate()
    {
        // SOLO ACTUALIZA SU MIRADA AL PLAYER CUANDO NO ESTÁ ATACANDO
        if(timeBetweeenUpdateLook <= 0)
        {
            _objMelee.transform.position = (Vector2)transform.position + CalculateMeleePosition();
            timeBetweeenUpdateLook = timeBetweenBase;
        }
        else { timeBetweeenUpdateLook -= Time.deltaTime; }
        

        // MODIFICAR TAG DEL OBJETO = ATAQUE MELEE
        if (_objMelee.isAttacking) { _objMelee.tag = "EnemyAttack"; }
        else { _objMelee.tag = "Untagged"; }
    }
    public override void SpecificAttack(int index = -1) { _anim.SetBool("Attack", true); }
    // ---- FUNCIÓN DE EVENTO DE ANIMACIÓN ---- //
    public void InitialAttack() { _objMelee.isAttacking = true; }
    public void FinishAttack() { _anim.SetBool("Attack", false); _objMelee.isAttacking = false; }
    // ---- FUNCIÓN INTEGRA ---- //
    private Vector2 CalculateMeleePosition()
    {
        Vector2 position = -(transform.position - _playerStats.transform.position);
        position.Normalize();

        position *= distanceMelee;

        return position;
    }
}