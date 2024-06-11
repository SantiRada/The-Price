using System.Collections;
using UnityEngine;

public enum SpreadHarm { receivedDamage, launched }
public class SpreadDamage : MonoBehaviour {

    [Header("Data Spread")]
    public GameObject objSpread;
    [Space]
    public SpreadHarm typeSpread;
    public TypeEnemyAttack typeAttackSpread;
    public float radioSpread;
    [Range(0, 0.25f), Tooltip("Valores menores equivalen a un crecimiento más rápido")] public float speedMovement;
    public bool healthPlayer;

    [Header("Data State")]
    public bool hasState;
    public TypeState state;
    public int countOfLoads;

    private GameObject spread;

    private void Start()
    {
        if (typeSpread == SpreadHarm.receivedDamage) PlayerStats.takeDamage += SpreadTypeDamage;
        else if (typeSpread == SpreadHarm.launched) SpreadTypeDamage();
    }
    private void SpreadTypeDamage()
    {
        if(typeAttackSpread != TypeEnemyAttack.Base)
        {
            // VERIFICAR QUE EL DAÑO RECIBIDO SEA DEL TIPO QUE PODÉS ESPARCIR, SINO RETURN
        }

        spread = Instantiate(objSpread, transform.position, Quaternion.identity);
        
        if (hasState)
        {
            spread.GetComponent<ObjectSpread>().state = state;
            spread.GetComponent<ObjectSpread>().countOfLoads = countOfLoads;
        }

        StartCoroutine("MoveSpread");
    }
    private IEnumerator MoveSpread()
    {
        while (Vector3.Distance(Vector3.one, spread.transform.localScale) < radioSpread)
        {
            spread.transform.localScale += new Vector3(0.2f, 0.2f, 0);
            yield return new WaitForSeconds(speedMovement);
        }

        if (healthPlayer) FindAnyObjectByType<PlayerStats>().SetValue(4, spread.GetComponent<ObjectSpread>().damageAffected);

        Destroy(spread);
        Destroy(gameObject);
    }
}
