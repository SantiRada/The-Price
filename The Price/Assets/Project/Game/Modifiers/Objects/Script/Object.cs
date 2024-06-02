using UnityEngine;

public enum TypeObject { basic, epic, legendary, mythical }
public abstract class Object : MonoBehaviour {

    [Header("Info Base")]
    public Sprite icon;
    public string itemName;
    public string description;
    public TypeObject typeObject;

    [Header("Triggers")]
    public bool finishRoom;
    public bool perfectRoom;
    public bool criticalAttack;
    public bool criticalDeath;
    public bool throwSkill;

    [Header("Player Content")]
    [HideInInspector] public PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    private void Start()
    {
        // ---- TRIGGERS ---- //
        if (finishRoom) RoomManager.finishRoom += Use;
        
        if (perfectRoom) RoomManager.perfectRoom += Use;
    }
    public abstract void Use();
}