using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    [Header("Elements of the UI")]
    [SerializeField] private Slider _progressBar;

    [Header("Static Elements")]
    [SerializeField] private static bool _inLoading = true;

    [Header("Data for Scene")]
    private string _currentScene;
    private static int _countElementInScene { get; set; }

    private SpriteRenderer _spr;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _inLoading = true;
        _currentScene = SceneManager.GetActiveScene().name;
        switch (_currentScene)
        {
            case "Menu": CountElement = 5; break;
        }

        _progressBar.value = 0;
        _progressBar.maxValue = CountElement;
    }
    private void Update()
    {
        if(_progressBar.value >= _progressBar.maxValue) _inLoading = false;

        _progressBar.value = CountElement;

        if (!_inLoading)
        {
            _spr.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), 0.5f);
            Invoke("DestroyElement", 1f);
        }
    }
    private void DestroyElement()
    {
        Destroy(gameObject);
    }
    // ---- SETTERS & GETTERS ---- //
    public static int CountElement
    {
        get { return _countElementInScene; }
        set {
            _countElementInScene = value;
        }
    }
}
