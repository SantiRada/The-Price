using System.Collections;
using UnityEngine;

public class FloatText : MonoBehaviour {

    public float timeInScreen;

    [Header("Movement")]
    [Range(0, 0.5f)] public float speedMovement;

    [Header("Shake")]
    public float shakeIntensity = 0.1f;
    public float rotationIntensity;
    private Vector3 originalPosition;

    private Animator anim;
    [HideInInspector] public bool isStatic = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        transform.SetAsFirstSibling();

        if (isStatic) { StartCoroutine("Shake"); }
        else { StartCoroutine("Movement"); }
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
    private IEnumerator Shake()
    {
        originalPosition = transform.localPosition;

        transform.rotation = Quaternion.Euler(0, 0, rotationIntensity);

        while (true)
        {
            Vector3 randomPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeIntensity;
            transform.localPosition = randomPosition;

            yield return new WaitForSeconds(0.05f);

            transform.localPosition = originalPosition;
        }
    }
}
