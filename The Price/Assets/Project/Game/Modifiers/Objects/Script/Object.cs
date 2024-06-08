using UnityEngine;

public enum TypeObject { basic, epic, legendary, mythical }
public enum TypeTrigger { perfectRoom }
public abstract class Object : ScriptableObject {

    [Header("Info Base")]
    public Sprite icon;
    public int itemName;
    public int description;
    public TypeObject typeObject;
    public bool canGet = true;

    [Header("Triggers")]
    public TypeTrigger trigger;

    [Header("0. PV\n1. Concentraci�n\n2. Velocidad de Movimiento\n3. Velocidad de Ataque\n4. Da�o de Habilidad\n5. Da�o\n6. Da�o Sucesivo\n7. Probabilidad de Cr�tico\n8. Probabilidad de Fallos\n9. Robo de Vida\n10. Cordura")]
    [Space]
    public float[] statsModifiable = new float[11];
    public float[] statsMaxModifiable = new float[11];

    [HideInInspector] public PlayerStats playerStats;

    public abstract void Use();
}