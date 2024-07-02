using UnityEngine;

public enum TypeCall { trigger, finishRoom, perfectRoom }
public class CallVoice : MonoBehaviour {

    [Header("Data Dialogue")]
    public int indexDialogue;
    public int repeatDialogue;
    public bool destroyer = false;
    private bool _spokeBefore = false;

    public TypeCall typeCall;
    private bool canAdvance = false;

    private void Start()
    {
        if(typeCall != TypeCall.perfectRoom) { RoomManager.finishRoom += AdvanceDialogue; }
        else { RoomManager.perfectRoom += AdvanceDialogue; }
    }
    private void AdvanceDialogue()
    {
        canAdvance = true;

        if(typeCall != TypeCall.trigger) { CreateDialogue(); }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canAdvance) { CreateDialogue(); }
    }
    private void CreateDialogue()
    {
        if (_spokeBefore) { VoiceSystem.StartDialogue(repeatDialogue); }
        else { VoiceSystem.StartDialogue(indexDialogue); }

        _spokeBefore = true;

        if (destroyer) { Destroy(this.gameObject); }
    }
    private void OnDestroy()
    {
        if (typeCall == TypeCall.perfectRoom) RoomManager.perfectRoom -= AdvanceDialogue;
        else if (typeCall == TypeCall.finishRoom) RoomManager.finishRoom -= AdvanceDialogue;
    }
}
