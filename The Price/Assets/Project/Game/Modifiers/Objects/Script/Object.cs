using UnityEngine;

public enum TypeObject { basic, epic, legendary, mythical }
public enum TypeTrigger { perfectRoom }
public abstract class Object : MonoBehaviour {

    [Header("Info Base")]
    public Sprite icon;
    public int itemName;
    public int description;
    public TypeObject typeObject;

    [Header("Triggers")]
    public TypeTrigger trigger;

    [Header("0. PV\n1. Concentración\n2. Velocidad de Movimiento\n3. Velocidad de Ataque\n4. Daño de Habilidad\n5. Daño\n6. Daño Sucesivo\n7. Probabilidad de Crítico\n8. Probabilidad de Fallos\n9. Robo de Vida\n10. Cordura")]
    [Space]
    public float[] statsModifiable = new float[11];
    public float[] statsMaxModifiable = new float[11];
    protected int position = 0;
    protected float value = 0;

    [Header("Player Content")]
    [HideInInspector] public PlayerStats playerStats;

    private void Awake() { playerStats = FindAnyObjectByType<PlayerStats>(); }
    public abstract void Use();
}