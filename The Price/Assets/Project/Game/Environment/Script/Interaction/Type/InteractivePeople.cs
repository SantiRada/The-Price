using UnityEngine;

public class InteractivePeople : InteractiveObject {

    private DialogueManager _dialogue;

    private void Start()
    {
        _dialogue = GetComponent<DialogueManager>();
    }
    public override void TakeAttack()
    {

    }
    public override void Select()
    {
        if (_dialogue._howToOpen == HowToOpenDialogue.RequiredAction)
        {
            _dialogue.ChangeState();
        }
    }
}
