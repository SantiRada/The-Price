using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionForControlPlayer;

public enum Speaker { Nash, Thanatos }
public class VoiceSystem : MonoBehaviour {

    [Header("Content UI")]
    public TextMeshProUGUI content;
    public Image speaker;
    public Image dialogue;
    [Space]
    public Sprite[] speakers;
    public Sprite[] dialogues;

    [Header("Content Show")]
    public Speaker[] typeSpeaker;
    public int[] message;
    private int index = 0;

    [Header("Private Data")]
    private bool inDialogue = false;
    private bool finishDialogue = false;
    private float timer = 1.5f;
    private CanvasGroup _canvas;

    private void Awake() { _canvas = GetComponent<CanvasGroup>(); }
    private void Start()
    {
        RoomManager.finishRoom += () => StartCoroutine("DialogueOn");

        CloseDialogue();
    }
    private void Update()
    {
        if (inDialogue)
        {
            if (finishDialogue)
            {
                timer -= Time.deltaTime;

                if (PlayerActionStates.IsUse || PlayerActionStates.IsDashing || PlayerActionStates.IsAttacking)
                {
                    if(timer <= 0) CloseDialogue();
                }
            }
            else { if (PlayerActionStates.IsUse || PlayerActionStates.IsDashing || PlayerActionStates.IsAttacking) Finish(); }
        }
    }
    public IEnumerator DialogueOn()
    {
        yield return new WaitForSeconds(0.01f);

        if (index < typeSpeaker.Length)
        {
            inDialogue = true;
            finishDialogue = false;
            content.text = "";
            timer = 1.5f;

            _canvas.interactable = true;
            _canvas.alpha = 1f;

            if (typeSpeaker[index] == Speaker.Nash)
            {
                speaker.sprite = speakers[0];
                dialogue.sprite = dialogues[0];
            }
            else
            {
                speaker.sprite = speakers[1];
                dialogue.sprite = dialogues[1];
            }

            string gameContent = LanguageManager.GetValue("Game", message[index]);
            Debug.Log(gameContent);

            char[] values = gameContent.ToCharArray();
            index++;

            for (int i = 0; i < values.Length; i++)
            {
                content.text += values[i];
                yield return new WaitForSeconds(0.05f);
            }

            Finish();
        }
    }
    private void Finish()
    {
        StopCoroutine("DialogueOn");

        content.text = LanguageManager.GetValue("Game", message[(index - 1)]);

        finishDialogue = true;
    }
    private void CloseDialogue()
    {
        inDialogue = false;
        finishDialogue = false;

        _canvas.alpha = 0f;
        _canvas.interactable = false;
    }
}
