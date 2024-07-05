
public class InteractiveWeapon : Interactive {

    private WeaponManagerUI _weaponManager;

    private void Start()
    {
        _weaponManager = FindAnyObjectByType<WeaponManagerUI>();

        RandomPool();
    }
    public override void OpenWindow()
    {
        inTrigger = true;

        _manager.LoadInfo((int)content, descContent, transform.position, nameContent, typeContent, false);
    }
    public override void RandomPool() { WeaponSystem weapon = _weaponManager.RandomPool(); }
    public override void Select()
    {
        _weaponManager.StartCoroutine("OpenWindow");

        inSelect = false;
        Destroy(gameObject);
    }
}