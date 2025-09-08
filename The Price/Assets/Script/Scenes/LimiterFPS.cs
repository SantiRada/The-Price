using TMPro;
using UnityEngine;

public enum LimitFPS { unlimited = 0, limit30 = 30, limit60 = 60, limit90 = 90, limit120 = 120 }
public class LimiterFPS : MonoBehaviour {

    [Header("FPS Data")]
    public bool showFPS;
    public LimitFPS limit;
    public TextMeshProUGUI textFPS;
    private float delta = 0.0f;
    private float prevFPS;

    [Header("Version Data")]
    public string version;
    public TextMeshProUGUI textVersion;

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
        if (showFPS)
        {
            delta += (Time.unscaledDeltaTime - delta) * 0.1f;
            float fps = 1.0f / delta;
            float value = Mathf.Ceil(fps);

            // Solo actualiza si la diferencia es de al menos 5 FPS respecto al último mostrado
            if (Mathf.Abs(value - prevFPS) >= 5f)
            {
                prevFPS = value;
                textFPS.text = value.ToString("0") + " FPS";
            }
        }
    }
}