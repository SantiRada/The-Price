using System;
using System.Collections;
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
    public static Action eventDialogue;

    private void Awake() { _canvas = GetComponent<CanvasGroup>(); }
    private void Start()
    {
        eventDialogue += () => StartCoroutine("DialogueOn");

        CloseDialogue();
    }
    public static void StartDialogue(int index)
    {
        indexDialogue = index;
        eventDialogue?.Invoke();
    }
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
    }
    public IEnumerator DialogueOn()
    {
        inDialogue = true;
        finishDialogue = false;
        content.text = "";
        
        timer = 1.5f;
        _canvas.alpha = 1f;

        char[] values = LanguageManager.GetValue("Game", indexDialogue).ToCharArray();

        for (int i = 0; i < values.Length; i++)
        {
            content.text += values[i];
            yield return new WaitForSeconds(0.05f);
        }

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
        inDialogue = false;
        finishDialogue = false;

        _canvas.alpha = 0f;
    }
}
