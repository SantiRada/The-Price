using UnityEngine;

public enum TypeContent { BigContent, SmallContent }
public abstract class CollectableInScene : MonoBehaviour {

    public TypeElement _typeElement;
    public TypeContent _typeContent;
    [HideInInspector] public CanvasGroup _windowAppear;

    private void Start()
    {
        if(_typeContent == TypeContent.BigContent) _windowAppear = GameObject.FindGameObjectWithTag("Collectable_Skill").GetComponent<CanvasGroup>();
        else _windowAppear = GameObject.FindGameObjectWithTag("Collectable_Aptitud").GetComponent<CanvasGroup>();

        HideWindow();
        InitialValues();
    }
    private void RepositionWindow()
    {
        _windowAppear.alpha = 1.0f;

        Vector3 positionWindow = Vector3.zero;
        if(_typeElement == TypeElement.Skills) positionWindow = transform.position + new Vector3(0, 1.65f, 0);
        else positionWindow = transform.position + new Vector3(0, 0.9f, 0);

        _windowAppear.transform.position = positionWindow;

        LoadData();
    }
    private void HideWindow()
    {
        _windowAppear.alpha = 0.0f;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) RepositionWindow();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Fire1")) Select();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) HideWindow();
    }
    public abstract void InitialValues();
    public abstract void Select();
    public abstract void LoadData();
}