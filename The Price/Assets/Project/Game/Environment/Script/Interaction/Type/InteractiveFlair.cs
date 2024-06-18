using UnityEngine;

public class InteractiveFlair : Interactive {

    [HideInInspector] public TypeFlair flair;
    [HideInInspector] public int amount;
    [HideInInspector] public TypeFlair affected;

    [Header("Content In Scene")]
    private FlairSystem _flairSystem;

    private void Start() { _flairSystem = FindAnyObjectByType<FlairSystem>(); }
    protected override void OpenWindow()
    {
        inTrigger = true;

        _manager.LoadInfo((int)content, descContent, transform.position, nameContent, typeContent, true, (priceInGold != 0) ? priceInGold.ToString() : "");
    }
    public override void RandomPool()
    {
        flair = _flairSystem.RandomFlairInSelector();
        amount = _flairSystem.CalculateAmount();
        affected = _flairSystem.RandomAffectedFlair(flair);

        _manager._nameContent[0].text = LanguageManager.GetValue("Game", 25) + " " + LanguageManager.GetValue("Game", (26 + (int)flair));

        _manager._descContent[1].text = _flairSystem.CreateContentDescription(flair, amount, affected);
    }
    public override void Select()
    {
        if (isShop)
        {
            Buy();
            return;
        }

        _flairSystem.StartCoroutine("Select");

        inSelect = false;
        Destroy(gameObject);
    }
}
