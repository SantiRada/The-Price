using UnityEngine;

public enum TypeCall { trigger, initialRoom, finishRoom, perfectRoom }
public class CallVoice : MonoBehaviour {

    [Header("Data Dialogue")]
    public int indexDialogue;
    public int repeatDialogue;
    [Space]
    public bool destroyer = false;
    public bool freeze = false;
    public bool requiredFinishRoom = false;
    public TypeCall typeCall;
    public Vector3 positionToCreate;

    [Header("Private Data")]
    private bool canAdvance = false;
    private bool _spokeBefore = false;

    private void Start()
    {
        if(typeCall == TypeCall.trigger)
        {
            if (requiredFinishRoom) RoomManager.finishRoom += AdvanceDialogue;
            else canAdvance = true;
        }
        else if(typeCall == TypeCall.finishRoom) { RoomManager.finishRoom += AdvanceDialogue; }
        else if (typeCall == TypeCall.initialRoom)
        {
            canAdvance = true;
            LoadingScreen.finishLoading += CreateDialogue;
        }
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
        if (_spokeBefore) { VoiceSystem.StartDialogue(repeatDialogue, freeze); }
        else { VoiceSystem.StartDialogue(indexDialogue, freeze); }

        _spokeBefore = true;

        if (destroyer) { Destroy(this.gameObject); }
    }
    private void OnDestroy()
    {
        if (typeCall == TypeCall.perfectRoom) RoomManager.perfectRoom -= AdvanceDialogue;
        else if (typeCall == TypeCall.finishRoom) RoomManager.finishRoom -= AdvanceDialogue;
        else if(typeCall == TypeCall.initialRoom) LoadingScreen.finishLoading -= CreateDialogue;
    }
}
