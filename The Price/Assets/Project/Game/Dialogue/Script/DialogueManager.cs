using UnityEngine;

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
    public bool _dialogueStatic = false;
    public bool _repeatDialogue = true;
    public bool _showSkills = false;
    public bool _showObject = false;
    public bool _showAptitud = false;
    [Space]
    private bool _inDialogue = false;
    private int _index = 0;
    private bool canRepeat = true;

    [Header("Private Content")]
    private DialogueUI _ui;

    private void Awake()
    {
        _ui = FindAnyObjectByType<DialogueUI>();
    }
    private void OnEnable()
    {
        if(_howToOpen == HowToOpenDialogue.RequiredStartRoom) ChangeState();
    }
    private void Start()
    {
        if(_howToOpen == HowToOpenDialogue.RequiredEndRoom) RoomManager.finishRoom += ChangeState;
    }
    public void ChangeState()
    {
        if (!canRepeat) return;

        _inDialogue = true;
        _index = 0;

        ShowDialogue();

        if (_dialogueStatic) Pause.StateChange = State.Interface;
        else Pause.StateChange = State.Game;

        if (!_repeatDialogue) canRepeat = false;
    }
    private void Update()
    {
        if (_inDialogue)
        {
            if (_ui.finishText)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    _index++;
                    ShowDialogue();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { if(_howToOpen == HowToOpenDialogue.RequiredTrigger) ChangeState(); }
    }
    private void ShowDialogue()
    {
        if(whatSay.Length > _index) { _ui.ShowDialogue(transform.position, _typeDialogue, whoSpeak, whatSay[_index]); }
        else
        {
            _inDialogue = false;
            _ui.hideDialogue();

            // PARA MOSTRAR EL SELECTOR DE HABILIDADES
            if (_showSkills)
            {
                SkillPlacement.StartSkillsSelector();

                Destroy(gameObject, 1f);
            }
            // PARA MOSTRAR EL SELECTOR DE OBJETOS
            if (_showObject)
            {
                ObjectPlacement.StartObjectSelector();

                Destroy(gameObject, 1f);
            }
        }
    }
}
