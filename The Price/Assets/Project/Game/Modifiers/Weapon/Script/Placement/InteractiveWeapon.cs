using UnityEngine;

public class InteractiveWeapon : Interactive {

    public WeaponSystem weaponPrev;
    private WeaponManagerUI _weaponManager;

    private void Start() { _weaponManager = FindAnyObjectByType<WeaponManagerUI>(); }
    public override void OpenWindow()
    {
        inTrigger = true;

        _manager.LoadInfo((int)content, descContent, transform.position, nameContent, typeContent, false);
    }
    public void SetWeapon(WeaponSystem weapon) { weaponPrev = weapon; }
    public override void RandomPool() { }
    public override void Select()
    {
        if (FindAnyObjectByType<Tutorial>()) { FindAnyObjectByType<Tutorial>().CallMadeInteraction(); }

        if(weaponPrev != null) _weaponManager.SetWeaponPool(weaponPrev);
        _weaponManager.StartCoroutine("OpenWindow");

        inSelect = false;
        Destroy(gameObject);
    }
}