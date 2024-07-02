using System;
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
    private float timer = 1.5f;
    private CanvasGroup _canvas;

    [Header("Static Data")]
    private static int indexDialogue;

    private void Awake() { _canvas = GetComponent<CanvasGroup>(); }
    private void OnEnable() { CloseDialogue(); }
    public static void StartDialogue(int index) { indexDialogue = index; }
    private void Update()
    {
        if (inDialogue)
        {
            if (finishDialogue)
            {
                timer -= Time.deltaTime;

                if (timer <= 0) CloseDialogue();
            }
            else { if (PlayerActionStates.IsUse || PlayerActionStates.IsDashing || PlayerActionStates.IsAttacking) Finish(); }
        }
        else
        {
            if(indexDialogue != -1) { StartCoroutine("DialogueOn"); }
        }
    }
    public IEnumerator DialogueOn()
    {
        inDialogue = true;
        finishDialogue = false;
        content.text = "";
        
        timer = 1.5f;
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

        Finish();
    }
    private void Finish()
    {
        StopCoroutine("DialogueOn");

        content.text = LanguageManager.GetValue("Game", indexDialogue);

        finishDialogue = true;
    }
    private void CloseDialogue()
    {
        indexDialogue = -1;
        inDialogue = false;
        finishDialogue = false;

        _canvas.alpha = 0f;
    }
}
