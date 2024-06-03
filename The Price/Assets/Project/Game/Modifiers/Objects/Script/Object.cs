using UnityEngine;

public enum TypeObject { basic, epic, legendary, mythical }
public enum TypeTrigger { finishRoom, perfectRoom, criticalAttack, criticalDeath, throwSkill }
public abstract class Object : MonoBehaviour {

    [Header("Info Base")]
    public Sprite icon;
    public int itemName;
    public int description;
    public TypeObject typeObject;

    [Header("Triggers")]
    public TypeTrigger trigger;
    public TypePermanenceOfObject _typePermanence;
    public float waitForPermanence;

    [Header("Player Content")]
    [HideInInspector] public PlayerStats playerStats;

    private void Awake() { playerStats = FindAnyObjectByType<PlayerStats>(); }
    private void Start()
    {
        // ---- TRIGGERS ---- //
        if (trigger == TypeTrigger.finishRoom) RoomManager.finishRoom += Use;
        
        if (trigger == TypeTrigger.perfectRoom) RoomManager.perfectRoom += Use;
    }
    public abstract void Use();
}