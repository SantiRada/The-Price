using UnityEngine;

public enum TypeTrigger { perfectRoom, fourKills, kills }
public enum TypeObject { basic, epic, legendary, mythical }
public enum TypeCanceled { permanent, receivedDamage, perRoom, perTime }
public abstract class Object : ScriptableObject {

    [Header("Info Base")]
    public Sprite icon;
    public int itemName;
    public int description;
    public TypeObject typeObject;

    [Header("Triggers")]
    public TypeTrigger trigger;
    public TypeCanceled typeCanceled;

    [Header("Cooling")]
    public bool hasCooling;
    public float timerCooling;
    protected float timerBaseCooling;
    [HideInInspector] public bool canActive = true;

    [Header("0. PV\n1. Concentraci�n\n2. Velocidad de Movimiento\n3. Velocidad de Ataque\n4. Da�o de Habilidad\n5. Da�o\n6. Da�o Sucesivo\n7. Probabilidad de Cr�tico\n8. Probabilidad de Fallos\n9. Robo de Vida\n10. Cordura")]
    [Space]
    public float[] statsModifiable = new float[11];
    public float[] statsMaxModifiable = new float[11];

    [HideInInspector] public PlayerStats playerStats;

    public void Cooling()
    {
        if (hasCooling)
        {
            timerCooling -= Time.deltaTime;

            if(timerCooling <= 0)
            {
                timerCooling = timerBaseCooling;
                canActive = true;
            }
        }
    }
    public abstract void Use();
    public abstract void CancelContent();
}