using System.Collections;
using UnityEngine;

public abstract class WeaponSystem : MonoBehaviour {

    [Header("Data Attack")]
    public float delayAttack = 0.25f;
    [HideInInspector] public uint countAttack = 0;
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool inAttack = false;
    [Space]
    public float delayDetectAttack = 0.6f;
    [HideInInspector] public float delayDetectBase;
    [HideInInspector] public bool inDetect = false;

    private void Update()
    {
        if (inDetect) delayDetectAttack -= Time.deltaTime;

        if(delayDetectAttack <= 0 && inDetect)
        {
            delayDetectAttack = delayDetectBase;
            inDetect = false;
            countAttack = 0;
        }
    }
    public void CallAttack()
    {
        if (inAttack || !canAttack) return;

        StartCoroutine("AttackTimer");
    }
    private IEnumerator AttackTimer()
    {
        canAttack = false;
        inAttack = true;

        countAttack++;

        inDetect = true;
        delayDetectAttack = delayDetectBase;

        Attack();
        Combo();

        yield return new WaitForSeconds(delayAttack);

        canAttack = true;
        inAttack = false;
    }
    public abstract void Attack();
    public abstract void Combo();
}
