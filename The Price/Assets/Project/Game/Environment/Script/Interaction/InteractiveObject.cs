using UnityEngine;
using static ActionForControlPlayer;

public enum ContentUI { skillContent, bigContent, smallContent, dialogueContent }
public enum ListContent { Menu, Game, Skill }
public abstract class InteractiveObject : MonoBehaviour {

    [Header("Content for the UI")]
    public ContentUI content;
    public ListContent typeContent;
    public int nameContent;
    public int descContent;
    public int attackContent;
    protected bool _wasAttacked = false;
    [HideInInspector] public bool inSelect = false;
    [HideInInspector] public bool inTrigger = false;

    [Header("Private Content")]
    [HideInInspector] public InteractiveManager _manager;

    private void OnEnable()
    {
        _manager = FindAnyObjectByType<InteractiveManager>();
    }
    protected void OpenWindow()
    {
        inTrigger = true;

        if(content == ContentUI.bigContent)
        {
            if (_wasAttacked) _manager.LoadInfo(0, attackContent, transform.position, nameContent, typeContent);
            else _manager.LoadInfo(0, descContent, transform.position, nameContent, typeContent);
        }
        else if (content == ContentUI.skillContent)
        {
            _manager.LoadInfo(1, descContent, transform.position, nameContent, typeContent);
            inSelect = true;
        }
        else if (content == ContentUI.smallContent || content == ContentUI.dialogueContent)
        {
            if(content == ContentUI.dialogueContent) if (gameObject.GetComponent<DialogueManager>()._howToOpen != HowToOpenDialogue.RequiredAction) return;

            if (_wasAttacked) _manager.LoadInfo(2, attackContent, transform.position, nameContent, typeContent);
            else _manager.LoadInfo(2, descContent, transform.position, nameContent, typeContent);
        }
    }
    protected void CloseWindow()
    {
        _manager.CloseWindow();

        inTrigger = false;

        if(content == ContentUI.skillContent) inSelect = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) OpenWindow();

        if (collision.CompareTag("Weapon") || collision.CompareTag("Proyectile")) TakeAttack();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerActionStates.IsUse && !inSelect)
            {
                inSelect = true;
                Select();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CloseWindow();
        }
    }
    // ---------------------- //
    public abstract void TakeAttack();
    public abstract void Select();
}