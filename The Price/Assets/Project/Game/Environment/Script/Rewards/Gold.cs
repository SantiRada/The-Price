using System.Collections;
using UnityEngine;

public class Gold : MonoBehaviour {

    [Header("Data")]
    [Tooltip("Cuanto tarda en empezar la animación del oro")] public float delayToAnim;
    [SerializeField, Tooltip("Cuanto dura la animación del oro")] public float _offsetToAnim;
    private bool canMove = false;
    [HideInInspector] public Transform target;

    private void Start()
    {
        StartCoroutine("InitAnimation");
    }
    private void FixedUpdate()
    {
        if (LoadingScreen.inLoading) return;

        if (canMove && !Pause._inPause) transform.position = Vector3.Lerp(transform.position, target.position, _offsetToAnim * Time.fixedDeltaTime);
    }
    private IEnumerator InitAnimation()
    {
        canMove = false;
        yield return new WaitForSeconds(delayToAnim);
        canMove = true;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
