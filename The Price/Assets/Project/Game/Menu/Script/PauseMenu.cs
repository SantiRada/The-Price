using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    [Header("Data UI")]
    [SerializeField] private GameObject _pauseWindow;
    [SerializeField] private Selectable _menuObject;

    [Header("Confirmation")]
    [SerializeField] private GameObject _confirmWindow;
    [SerializeField] private Selectable _objConfirmation;

    [Header("Settings")]
    [SerializeField] private GameObject _settingsWindow;

    [Header("Data Pause")]
    private float _timer = 0.3f;
    private bool _canChangePause = true;
    private static bool inPause { get; set; }

    private Settings _settings;

    private void Awake()
    {
        _settings = GetComponentInChildren<Settings>();
    }
    private void Start()
    {
        Pause = false;
        _pauseWindow.SetActive(inPause);
        _confirmWindow.SetActive(false);
        _settingsWindow.SetActive(false);
    }
    private void Update()
    {
        // CERRAR SETTINGS Y VOLVER AL MENU BASE
        if ((Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape)) && _settings.Config)
        {
            CloseSettings();
        }

        if (!_canChangePause)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0) _canChangePause = true;
        }
    }
    private void ChangePause()
    {
        _pauseWindow.SetActive(inPause);
        _canChangePause = false;
        _timer = 0.3f;

        if (Pause)
        {
            _menuObject.Select();
        }
    }
    // ---- BUTTONS ---- //
    public void ContinueGame()
    {
        Pause = false;
    }
    // ----------------- //
    public void GoToSettings()
    {
        _settingsWindow.SetActive(true);

        _settings.OpenConfig();
    }
    public void CloseSettings()
    {
        _settings.CloseConfig();

        _settingsWindow.SetActive(false);
        _menuObject.Select();
    }
    // ----------------- //
    public void BackToMenu()
    {
        _confirmWindow.SetActive(true);
        _objConfirmation.Select();
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void CloseConfirmation()
    {
        _confirmWindow.SetActive(false);
        _menuObject.Select();
    }
    // ----------------- //
    public void QuitGame()
    {
        Debug.Log("Se cerró el juego :(");
        Application.Quit();
    }
    // ---- SETTERS Y GETTERS ---- //
    public bool Pause
    {
        get { return inPause; }
        set {
            if (!_canChangePause) return;

            inPause = value;
            ChangePause();
        }
    }
}