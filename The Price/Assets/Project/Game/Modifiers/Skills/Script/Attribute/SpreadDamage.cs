using System.Collections;
using UnityEngine;

public enum SpreadHarm { receivedDamage, launched, Attack }
public class SpreadDamage : MonoBehaviour {

    [Header("Data Spread")]
    public GameObject objSpread;
    [Space]
    public SpreadHarm typeSpread;
    public TypeEnemyAttack typeAttackSpread;
    public float radioSpread;
    [Range(0, 0.25f), Tooltip("Valores menores equivalen a un crecimiento m�s r�pido")] public float speedMovement;
    public bool healthPlayer;

    [Header("Data State")]
    public bool hasState;
    public TypeState state;
    public int countOfLoads;

    [HideInInspector] public GameObject spread;

    private void Start()
    {
        if (typeSpread == SpreadHarm.receivedDamage) PlayerStats.takeDamage += SpreadTypeDamage;
        else if (typeSpread == SpreadHarm.launched) SpreadTypeDamage();
    }
    public void SpreadTypeDamage()
    {
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
    }
}
