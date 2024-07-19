using TMPro;
using UnityEngine;

public enum LimitFPS { unlimited = 0, limit30 = 30, limit60 = 60, limit90 = 90, limit120 = 120 }
public class LimiterFPS : MonoBehaviour {

    [Header("FPS Data")]
    public bool showFPS;
    public LimitFPS limit;
    public TextMeshProUGUI textFPS;
    private float delta = 0.0f;

    [Header("Version Data")]
    public string version;
    public TextMeshProUGUI textVersion;
    private float timerToVerifyFPS = 1f;

    private void Start()
    {
        VerifyFPS();

        textVersion.text = version;
    }
    private void VerifyFPS()
    {
        if (showFPS) textFPS.gameObject.SetActive(true);
        else textFPS.gameObject.SetActive(false);

        Application.targetFrameRate = (int)limit;
    }
    private void Update()
    {
        timerToVerifyFPS -= Time.deltaTime;

        if (showFPS && timerToVerifyFPS <= 0)
        {
            delta += (Time.unscaledDeltaTime - delta) * 0.1f;
            float fps = 1.0f / delta;
            textFPS.text = Mathf.Ceil(fps).ToString() + " FPS";

            timerToVerifyFPS = 1f;
        }
    }
}
