using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using static ActionForControlPlayer;

public class VoiceSystem : MonoBehaviour {

    [Header("Content UI")]
    public TextMeshProUGUI content;

    [Header("Private Data")]
    private bool inDialogue = false;
    private bool finishDialogue = false;
    private CanvasGroup _canvas;

    [Header("Static Data")]
    private static int indexDialogue;
    private static bool freeze = false;

    private void Awake() { _canvas = GetComponent<CanvasGroup>(); }
    private void OnEnable() { CloseDialogue(); }
    public static void StartDialogue(int index, bool freezeGame = false)
    {
        indexDialogue = index;
        freeze = freezeGame;

        if (freeze) { CameraMovement.SetSize(SizeCamera.specific); }
    }
    private void Update()
    {
        if (inDialogue)
        {
            if (PlayerActionStates.IsDashing || PlayerActionStates.IsUse)
            {
                if(finishDialogue) CloseDialogue();
                else StartCoroutine("Finish");
            }
        }
        else
        {
            if(indexDialogue != -1) { StartCoroutine("DialogueOn"); }
        }
    }
    public IEnumerator DialogueOn()
    {
        if (freeze)
        {
            Pause.state = State.Interface;
            CameraMovement.SetCinematic(true);
        }
        inDialogue = true;
        finishDialogue = false;
        content.text = "";
       
        _canvas.alpha = 1f;

        string dialogue = LanguageManager.GetValue("Game", indexDialogue);
        StringBuilder visibleText = new StringBuilder();
        StringBuilder fullText = new StringBuilder();
        bool insideTag = false;

        for (int i = 0; i < dialogue.Length; i++)
        {
            char c = dialogue[i];

            if (c == '<') { insideTag = true; }

            if (!insideTag)
            {
                visibleText.Append(c);
                content.text = visibleText.ToString();
                yield return new WaitForSeconds(0.05f);
            }

            fullText.Append(c);

            if (c == '>')
            {
                content.text = fullText.ToString();
                insideTag = false;
            }
        }

        // Asigna el texto completo (con etiquetas) al componente de texto
        content.text = fullText.ToString();

        StartCoroutine("Finish");
    }
    private IEnumerator Finish()
    {
        StopCoroutine("DialogueOn");

        if(indexDialogue > 0) content.text = LanguageManager.GetValue("Game", indexDialogue);

        yield return new WaitForSeconds(0.75f);

        finishDialogue = true;
    }
    private void CloseDialogue()
    {
        indexDialogue = -1;
        inDialogue = false;
        finishDialogue = false;

        _canvas.alpha = 0f;

        if (freeze)
        {
            Pause.state = State.Game;
            CameraMovement.SetCinematic(false);
            CameraMovement.SetSize(SizeCamera.normal);
        }
    }
}
