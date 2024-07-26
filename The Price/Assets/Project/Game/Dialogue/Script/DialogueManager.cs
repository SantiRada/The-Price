using System.Collections;
using UnityEngine;
using static ActionForControlPlayer;

public enum TypeDialogue { FullScreen, Window }
public enum HowToOpenDialogue { RequiredTrigger, RequiredAction, RequiredStartRoom, RequiredEndRoom }
public class DialogueManager : MonoBehaviour {

    [Header("Data Dialogue")]
    public TypeDialogue _typeDialogue;
    public HowToOpenDialogue _howToOpen;
    [Space]
    public int whoSpeak;
    public int[] whatSay;

    [Header("Data")]
    public bool callerCamera = false;
    public bool dialogueStatic = false;
    public bool repeatDialogue = true;
    public bool randomDialogue = false;
    [Space]
    public bool showSkills = false;
    public bool showObject = false;
    public bool showAptitud = false;
    [Space]
    private bool _inDialogue = false;
    private bool canRepeat = true;
    private int _index = 0;

    [Header("Private Content")]
    private DialogueUI _ui;

    private void Awake() { _ui = FindAnyObjectByType<DialogueUI>(); }
    private void OnEnable() { if (_howToOpen == HowToOpenDialogue.RequiredStartRoom) ChangeState(); }
    private void Start()
    {
        if (_howToOpen == HowToOpenDialogue.RequiredEndRoom) RoomManager.finishRoom += ChangeState;

        if (showSkills || showObject) StartCoroutine("CrazyPeople");
    }
    private IEnumerator CrazyPeople()
    {
        string[] letter = { "E=mc?", "F=ma", "V=IR", "E=hf", "d=vt", "Q=It", "E=Vq", "w=fd", "c=nrt", "s=vt", "p=mv" };
        int levelCrazy = Random.Range(1, 4);

        while (true)
        {
            for (int i = 0; i < levelCrazy; i++) { FloatTextManager.CreateText(transform.position, TypeColor.Crazy, letter[Random.Range(0, letter.Length)], true); }
            yield return new WaitForSeconds(0.75f);
        }
    }
    public void ChangeState()
    {
        if (!canRepeat) return;

        _inDialogue = true;
        _index = 0;

        ShowDialogue();

        if (dialogueStatic) Pause.StateChange = State.Interface;
        else Pause.StateChange = State.Game;

        if (!repeatDialogue) canRepeat = false;
    }
    private void Update()
    {
        if (_inDialogue)
        {
            if (_ui.finishText)
            {
                if (PlayerActionStates.IsUse)
                {
                    _index++;
                    ShowDialogue();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { if (_howToOpen == HowToOpenDialogue.RequiredTrigger) ChangeState(); }

        if (collision.CompareTag("Proyectile")) if (_howToOpen == HowToOpenDialogue.RequiredTrigger && randomDialogue) ChangeState();
    }
    private void ShowDialogue()
    {
        if (randomDialogue) _index = Random.Range(0, whatSay.Length);

        if (whatSay.Length > _index)
        {
            if (_typeDialogue == TypeDialogue.Window) if (callerCamera) CameraMovement.CallCamera(transform.position, 1000);
            _ui.ShowDialogue(transform.position, _typeDialogue, whoSpeak, whatSay[_index]);
        }
        else
        {
            if (callerCamera) CameraMovement.CancelCallCamera();

            _inDialogue = false;
            _ui.hideDialogue();

            // PARA MOSTRAR EL SELECTOR DE HABILIDADES
            if (showSkills) SkillPlacement.StartSkillsSelector();

            // PARA MOSTRAR EL SELECTOR DE OBJETOS
            if (showObject) ObjectPlacement.StartObjectSelector();

            // PARA MOSTRAR EL SELECTOR DE INSTINTOS
            if (showAptitud) FlairSystem.StartFlairSelector();

            Destroy(gameObject, 0.65f);
        }
    }
}