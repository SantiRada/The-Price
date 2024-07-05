using UnityEngine;

public class InteractivePeople : Interactive {

    private DialogueManager _dialogue;

    private void Start() { _dialogue = GetComponent<DialogueManager>(); }
    public override void RandomPool() { Debug.Log("No debo randomizar un di�logo..."); }
    public override void Select() { if(_dialogue != null) if (_dialogue._howToOpen == HowToOpenDialogue.RequiredAction) _dialogue.ChangeState(); }
}
