using System.Collections;
using UnityEngine;

public class FloatText : MonoBehaviour {

    [Range(0, 0.5f)] public float speedMovement;
    public float timeInScreen;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        transform.SetAsFirstSibling();

        StartCoroutine("Movement");
    }
    private void Update()
    {
        if (LoadingScreen.inLoading || Pause.inPause || Pause.state != State.Game) return;

        // DESVANECIMIENTO DEL TEXTO
        timeInScreen -= Time.deltaTime;
        if (timeInScreen < 0) anim.SetBool("Destroy", true);
    }
    public void DestroyObject() { Destroy(gameObject); }
    private IEnumerator Movement()
    {
        while (true)
        {
            transform.position += new Vector3(0, speedMovement, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
