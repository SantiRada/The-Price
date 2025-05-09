using UnityEngine;

public class SlotSkill : MonoBehaviour {

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void QuitAnimator()
    {
        anim.enabled = false;
    }
}
