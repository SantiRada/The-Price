using System.Collections;
using UnityEngine;

public enum TypeState { Null, Quema, Sangrado, Stun }
public class AffectedState : MonoBehaviour {

    public TypeState state;
    public int numberOfLoads;
    [Tooltip("Cada cuanto tiempo hace efecto el estado")] public float whenTakeEffect;
    [Space]
    public float timeForLoads;
    private float baseTime;

    private EnemyManager enemy;

    private void OnEnable()
    {
        enemy = GetComponent<EnemyManager>();

        baseTime = timeForLoads;
    }
    private void Update()
    {
        if(state != TypeState.Null)
        {
            timeForLoads -= Time.deltaTime;

            if(timeForLoads <= 0) ResetTimer();
        }
    }
    public void CreateState(TypeState st, int number)
    {
        state = st;
        numberOfLoads = number;

        if(state != TypeState.Null) StartCoroutine("TakeEffect");
    }
    private IEnumerator TakeEffect()
    {
        while(numberOfLoads > 0)
        {
            if(state == TypeState.Quema || state == TypeState.Sangrado)
            {
                // VERIFICACIONES DE LOS ESTADOS "QUEMA" Y "SANGRADO"
                int value;
                if (state == TypeState.Quema) value = 3;
                else value = 5;

                // APLICAR DAÑO AL ENEMIGO
                enemy.TakeDamage(value * numberOfLoads);
            }
            else if(state == TypeState.Stun)
            {
                // VERIFICACIONES DEL ESTADO "STUN"
                if (numberOfLoads > 1) enemy.CanMove = false;
                else if (numberOfLoads == 1) enemy.speed = (enemy.speed / 2);
            }
            yield return new WaitForSeconds(whenTakeEffect);
        }
    }
    private void FinishState() { Destroy(this); }
    // ---- FUNCIÓN INTEGRA ---- //
    private void ResetTimer()
    {
        // REDUCIR EL NÚMERO DE CARGAS
        if (numberOfLoads > 0) numberOfLoads -= 1;

        // VERIFICAR EL FUNCIONAMIENTO DEL ESTADO "STUN"
        if (state == TypeState.Stun)
        {
            if (!enemy.CanMove) enemy.CanMove = true;
            else enemy.speed = (enemy.speed * 2);
        }

        // RESTABLECER EL TIMER PARA LA SIGUIENTE ITERACION
        timeForLoads = baseTime;

        // VERIFICACIÓN FINAL PARA QUITAR EL ESTADO
        if(numberOfLoads <= 0) FinishState();
    }
}
