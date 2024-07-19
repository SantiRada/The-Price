
public class InteractiveWeapon : Interactive {

    public WeaponSystem weaponPrev;
    private WeaponManagerUI _weaponManager;

    private void Start()
    {
        _weaponManager = FindAnyObjectByType<WeaponManagerUI>();
    }
    public override void OpenWindow()
    {
        inTrigger = true;

        RandomPool();
        _manager.LoadInfo((int)content, descContent, transform.position, nameContent, typeContent, false);
    }
    public override void RandomPool()
    {
        if(weaponPrev != null) { _weaponManager.SetWeaponPool(weaponPrev); }
        else { _weaponManager.RandomPool(); }
    }
    public override void Select()
    {
        RoomManager.CallMadeInteraction();
        _weaponManager.StartCoroutine("OpenWindow");

        inSelect = false;
        Destroy(gameObject);
    }
}