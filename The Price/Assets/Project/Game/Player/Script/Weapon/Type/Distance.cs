using UnityEngine;

public enum DistanceFinalHit { Null, theyAreQuantity, theyAreBig, generateAreaDamage }
public class Distance : WeaponSystem {

    [Header("Distance Data")]
    public GameObject projectile;
    public float distanceAttack;
    public float speedProjectile;
    public bool canTraverse;

    [Header("Final HITs")]
    public DistanceFinalHit typeFinalHit;

    [Header("Private Data")]
    private CrosshairData _crosshair;

    private void Awake() { _crosshair = FindAnyObjectByType<CrosshairData>(); }
    public override void Attack() { CreateProjectile(false); }
    public override void FinalHit() { CreateProjectile(true); }
    // ---- FUNCION INTEGRA ---- //
    private void CreateProjectile(bool isFinalHit)
    {
        Projectile pr = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();
        Vector3 direction = _crosshair.GetCurrentAimDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (isFinalHit)
        {
            if(typeFinalHit == DistanceFinalHit.theyAreQuantity)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile proyectil = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();

                    proyectil.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    proyectil.SetterValues(_player.gameObject, distanceAttack, damage, canTraverse, direction, 0, speedProjectile);
                }
            }
            else if(typeFinalHit == DistanceFinalHit.theyAreBig)
            {
                pr.transform.localScale = new Vector3(pr.transform.localScale.x*2, pr.transform.localScale.y * 2, pr.transform.localScale.z * 2);
            }
            else if(typeFinalHit == DistanceFinalHit.generateAreaDamage) { pr.canAreaDamage = true; }
        }

        pr.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        pr.SetterValues(_player.gameObject, distanceAttack, damage, canTraverse, _crosshair.GetCurrentAimDirection(), 0, speedProjectile);
    }
}
