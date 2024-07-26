using UnityEngine;

public enum TypeChest { Gold, Souls, Bomb }
public class InteractiveChest : Interactive {

    public TypeChest typeChest;
    public int count;

    private Animator _anim;

    private void Awake() { _anim = GetComponent<Animator>(); }
    public override void RandomPool() { Debug.Log("No debo randomizar un cofre..."); }
    public override void Select()
    {
        if (Pause.state != State.Game) return;

        _anim.SetBool("Open", true);

        if(typeChest == TypeChest.Gold) { ManagerGold.CreateGold(transform.position, (CountGold)count); }
        if (typeChest == TypeChest.Souls) { ManagerGold.CreateSouls(transform.position, count); }

        inTrigger = false;
        CloseWindow();

        Destroy(this);
        Destroy(gameObject, 3f);
    }
}