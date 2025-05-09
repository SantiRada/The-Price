using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public class DialogueUI : MonoBehaviour {

    [Header("UI Content")]
    public GameObject contentUI;
    public Image character;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject clicContinue;

    [Header("UI World Space")]
    public GameObject window;
    public TextMeshProUGUI nameTextWS;
    public TextMeshProUGUI contentTextWS;
    public GameObject clicContinueWS;

    [Header("Private Info")]
    [SerializeField, Range(0, 0.25f)] private float _delayForLetter;
    [SerializeField] private bool _loadForWords;
    private TypeDialogue _typeDialogue;
    private int _currentName;
    private int _currentDialogue;
    private bool inLoad = false;
    [HideInInspector] public bool finishText = false;

    private void Start() { contentUI.gameObject.SetActive(false); }
    private void Update()
    {
        if (inLoad)
        {
            if (PlayerActionStates.IsUse)
            {
                StopAllCoroutines();
                AllContentLoad();
                inLoad = false;
            }
        }
    }
    public void ShowDialogue(Vector3 position, TypeDialogue type, int name, int dialogue)
    {
        finishText = false;
        _typeDialogue = type;
        _currentName = name;
        _currentDialogue = dialogue;

        if (type == TypeDialogue.Window)
        {
            window.SetActive(true);
            window.transform.position = position + new Vector3(0, 2, 0);
        }
        else { contentUI.SetActive(true); }

        StartCoroutine("LoadDialogue");
    }
    public void hideDialogue()
    {
        window.SetActive(false);
        contentUI.SetActive(false);

        Pause.StateChange = State.Game;
    }
    private IEnumerator LoadDialogue()
    {
        clicContinue.SetActive(false);

        string name = LanguageManager.GetValue("Game", _currentName);
        string content = LanguageManager.GetValue("Game", _currentDialogue);
        
        dialogueText.text = "";
        contentTextWS.text = "";

        if(_typeDialogue == TypeDialogue.Window) nameTextWS.text = name;
        else nameText.text = name;

        if (_loadForWords)
        {
            string[] words = content.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (_typeDialogue == TypeDialogue.Window) contentTextWS.text += words[i] + " ";
                else dialogueText.text += words[i] + " ";

                yield return new WaitForSeconds(_delayForLetter);
                if (i > 10) inLoad = true;
            }
        }
        else
        {
            for (int i = 0; i < content.Length; i++)
            {
                if (_typeDialogue == TypeDialogue.Window) contentTextWS.text += content[i];
                else dialogueText.text += content[i];

                yield return new WaitForSeconds(_delayForLetter);
                if (i > 10) inLoad = true;
            }
        }

        AllContentLoad();
    }
    private void AllContentLoad()
    {
        dialogueText.text = LanguageManager.GetValue("Game", _currentDialogue);
        inLoad = false;
        clicContinue.SetActive(true);
        clicContinueWS.SetActive(true);
        finishText = true;
    }
}
