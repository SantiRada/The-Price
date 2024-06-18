using UnityEngine;

public class InteractiveObject : Interactive {

    public Object obj;

    [Header("Private Content")]
    private ObjectPlacement _placement;

    private void Start()
    {
        _placement = FindAnyObjectByType<ObjectPlacement>();
    }
    public override void RandomPool()
    {
        obj = _placement.RandomPool();

        nameContent = obj.itemName.ToString();
        descContent = obj.description.ToString();
    }
    public override void Select()
    {
        if (isShop)
        {
            Buy();
            return;
        }

        _player.objects.Add(obj);

        inSelect = false;
        Destroy(gameObject);
    }
}
