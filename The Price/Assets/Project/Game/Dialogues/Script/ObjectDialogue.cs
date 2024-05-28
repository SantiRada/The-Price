using UnityEngine;

public enum TypeAngry { Basic, Enemy, getAway, curse }
public class ObjectDialogue : InteractiveObject {

    [SerializeField] private bool _showSkills = false;
    [SerializeField] private bool _useDialogueSystem;
    [SerializeField] private int[] _phrases;
    private int _index = 0;

    [SerializeField] private int _counterForAngry;
    [SerializeField] private TypeAngry _type;

    private DialogueSystem _dialogue;

    private void Awake()
    {
        _dialogue = FindAnyObjectByType<DialogueSystem>();
    }
    private void VerifyAngry()
    {
        if(_counterForAngry <= 0)
        {
            switch (_type)
            {
                case TypeAngry.Enemy: ChangeType(); break;
                case TypeAngry.getAway: GetAway(); break;
                case TypeAngry.curse: Debug.Log("¡Te maldigo!"); break;
            }
        }
    }
    private void ChangeType()
    {
        Debug.Log("Se volvió un enemigo.");
        
        gameObject.AddComponent<Arvis>();
        enabled = false;
    }
    private void GetAway()
    {
        Debug.Log("Se enojó...");
        gameObject.SetActive(false);
    }
    public override void TakeAttack()
    {
        _counterForAngry--;
        VerifyAngry();
    }
    public override void Select()
    {
        Debug.Log("Select");
        if (_showSkills)
        {
            SkillPlacement.StartSkillsSelector();
            Destroy(gameObject, 1.5f);
        }

        if (_useDialogueSystem)
        {
            _dialogue.Active(nameContent, _phrases);
            return;
        }

        if(_phrases.Length > 1)
        {
            _index++;
            descContent = _phrases[_index];
        }
    }
}
