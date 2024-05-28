using UnityEngine;

public enum ContentUI { bigContent, smallContent }
public abstract class InteractiveObject : MonoBehaviour {

    [Header("Content for the UI")]
    public ContentUI _content;
    public int nameContent;
    public int descContent;
    public int attackContent;
    protected bool _wasAttacked = false;
    private bool inSelect = false;

    [Header("Private Content")]
    private InteractiveManager _manager;

    private void Start()
    {
        _manager = FindAnyObjectByType<InteractiveManager>();
    }
    private void OpenWindow()
    {
        if(_content == ContentUI.bigContent)
        {
            if (_wasAttacked) _manager.LoadBigInfo(nameContent, attackContent, transform.position);
            else _manager.LoadBigInfo(nameContent, descContent, transform.position);
        }
        else
        {
            if(_wasAttacked) _manager.LoadSmallInfo(attackContent, transform.position);
            else _manager.LoadSmallInfo(descContent, transform.position);
        }
    }
    private void CloseWindow()
    {
        _manager.CloseWindow();
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
            if (Input.GetButton("Fire1") && !inSelect)
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