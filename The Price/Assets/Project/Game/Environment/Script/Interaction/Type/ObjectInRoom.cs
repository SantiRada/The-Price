using UnityEngine;

public class ObjectInRoom : InteractiveObject {

    public Object obj;

    [Header("Private Content")]
    private ObjectPlacement _placement;
    private PlayerMovement _player;
    [HideInInspector] public bool isNew;

    private void Start()
    {
        _placement = FindAnyObjectByType<ObjectPlacement>();
        _player = FindAnyObjectByType<PlayerMovement>();

        if (isNew) ChangeSkill(_placement.RandomPool());
    }
    public void ChangeSkill(Object objectBase)
    {
        obj = objectBase;

        nameContent = obj.itemName;
        descContent = obj.description;
    }
    public override void TakeAttack() { Debug.Log("No ocurre nada ante el golpe"); }
    public override void Select()
    {
        _player.objects.Add(obj);

        inSelect = false;
        Destroy(gameObject);
    }
}
