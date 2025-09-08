using System.Collections;
using UnityEngine;

public class Gold : MonoBehaviour {

    [Header("Data")]
    [Tooltip("Cuanto tarda en empezar la animación del oro")] public float delayToAnim;
    [SerializeField, Tooltip("Cuanto dura la animación del oro")] public float _offsetToAnim;
    private bool canMove = false;

    private void Start() { StartCoroutine("InitAnimation"); }
    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 screenTopRight = new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane);
            Vector3 worldTopRight = Camera.main.ScreenToWorldPoint(screenTopRight) + new Vector3(-2, 2, 0);

            transform.position = Vector3.Lerp(transform.position, worldTopRight, _offsetToAnim * Time.fixedDeltaTime);
        }
    }
    private IEnumerator InitAnimation()
    {
        canMove = false;
        yield return new WaitForSeconds(delayToAnim);
        canMove = true;
        yield return new WaitForSeconds(1.65f);
        Destroy(gameObject);
    }
}