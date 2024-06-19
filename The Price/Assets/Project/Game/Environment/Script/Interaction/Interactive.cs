using UnityEngine;
using static ActionForControlPlayer;

public abstract class Interactive : MonoBehaviour {

    [Header("Content for the UI")]
    public ContentUI content;
    public ListContent typeContent;
    public string nameContent;
    public string descContent;
    [HideInInspector] public bool inSelect = false;
    [HideInInspector] public bool inTrigger = false;
    [HideInInspector] public bool isNew = false;
    [HideInInspector] public bool isShop = false;
    [HideInInspector] public int priceInGold;

    [Header("Private Content")]
    [HideInInspector] public InteractiveManager _manager;
    protected PlayerStats _player;
    protected HUD _HUD;

    private void OnEnable()
    {
        _manager = FindAnyObjectByType<InteractiveManager>();
        _player = FindAnyObjectByType<PlayerStats>();
        _HUD = FindAnyObjectByType<HUD>();

        if (isNew && !isShop) RandomPool();
    }
    public virtual void OpenWindow()
    {
        inTrigger = true;

        // CANCELAR LA ABERTURA DE LA VENTANA SI TIENE UN TRIGGER ESPECÍFICO DISTINTO
        if (content == ContentUI.dialogueContent) if (gameObject.GetComponent<DialogueManager>()._howToOpen != HowToOpenDialogue.RequiredAction) return;

        _manager.LoadInfo((int)content, descContent, transform.position, nameContent, typeContent, false, (priceInGold != 0) ? priceInGold.ToString() : "");
    }
    protected void CloseWindow() { _manager.CloseWindow(content, isShop); }
    private void OnTriggerEnter2D(Collider2D collision) { if (collision.CompareTag("Player")) OpenWindow(); }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerActionStates.IsUse && !inSelect)
            {
                inTrigger = true;
                inSelect = true;
                Select();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inTrigger = false;
            CloseWindow();
        }
    }
    public abstract void RandomPool();
    public abstract void Select();
    public void Buy()
    {
        if(_HUD.countFinishGold >= priceInGold)
        {
            // PUEDO COMPRAR
            _HUD.SetGold(-priceInGold);
            
            isShop = false;

            if (GetComponent<InteractiveSkill>())
            {
                gameObject.AddComponent<Bounce>();

                CloseWindow();
                content = ContentUI.skillContent;
                OpenWindow();
            }
            else { Select(); }
        }
        else { Debug.Log("Este artículo no alcanza"); }
    }
}
