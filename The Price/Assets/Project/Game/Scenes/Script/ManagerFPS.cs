using TMPro;
using UnityEngine;

public enum Limiter { notLimit = 0, limit30 = 30, limit60 = 60, limit90 = 90, limit120 = 120 }
public class ManagerFPS : MonoBehaviour {

    [Header("Data Version")]
    [SerializeField] private TextMeshProUGUI _versionText;
    [SerializeField] private string _versionGame;


    [Header("Data FPS")]
    [SerializeField] private Limiter _limit;
    [SerializeField] private TextMeshProUGUI _fpsText;

    [Header("Private Data")]
    private float deltaTime = 0.0f;

    private void Start()
    {
        Application.targetFrameRate = (int)_limit;
        _versionText.text = _versionGame;
    }
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        _fpsText.text = string.Format("{0:0.} FPS", fps);
    }
}
