using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TypeController { Keyboard, XBox, PlayStation }
public class InputManager : MonoBehaviour {

    [Header("UI Controller")]
    [SerializeField] private Sprite[] xboxUI;
    [SerializeField] private Sprite[] playStationUI;
    [SerializeField] private Sprite[] keyboardMouseUI;
    [Space]
    [SerializeField] private TypeController _typeController;

    [Header("Detector Players")]
    public List<string> _players = new List<string>();
    [SerializeField] private int _countPlayerCreated = 0;

    [Header("Content Game")]
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _hudPlayer;
    [SerializeField] private GameObject _statsPlayer;
    [HideInInspector] public string _currentScene;

    private void Start()
    {
        _currentScene = SceneManager.GetActiveScene().name;

        if (_currentScene.Contains("Menu") || _currentScene.Contains("Testing")) return;

        string[] data = PlayerPrefs.GetString("dataPlayers", "Keyboard & Mouse").Split(',');

        for(int i = 0; i < PlayerPrefs.GetInt("countPlayers", 1); i++)
        {
            _players.Add(data[i]);
        }

        CreatePlayersInScene();
    }
    private void CreatePlayersInScene()
    {
        Transform parentHUD = GameObject.FindGameObjectWithTag("HUD").transform;
        Transform parentStats = GameObject.FindGameObjectWithTag("UI").transform;

        for (int i = 0; i < _players.Count; i++)
        {
            // ---- PLAYER -------------------- //
            GameObject obj = Instantiate(_playerObj, new Vector3(i, 0, 0), Quaternion.identity);
            PlayerStats pj = obj.GetComponent<PlayerStats>();

            // ---- HUD ----------------------- //
            GameObject objHUD = Instantiate(_hudPlayer, Vector3.zero, Quaternion.identity, parentHUD);
            objHUD.transform.position = Vector3.zero;
            
            // ---- STATS --------------------- //

            GameObject objStats = Instantiate(_statsPlayer, Vector3.zero, Quaternion.identity, parentStats);
            #region RepositionStats
            // Obtén el RectTransform del Canvas
            RectTransform canvasRect = parentStats.GetComponent<RectTransform>();

            // Obtén el RectTransform del objeto objStats
            RectTransform statsRect = objStats.GetComponent<RectTransform>();

            // Establece la posición en el centro del Canvas (0,0)
            Vector2 centerPosition = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, centerPosition, null, out Vector2 anchoredPosition);

            // Establece la posición del objeto objStats en el (0,0) relativo al Canvas
            statsRect.anchoredPosition = anchoredPosition;
            #endregion

            pj.SetUI(objHUD, objStats, _players[i]);
            _countPlayerCreated = i;
        }
    }
    public Sprite GetSpriteControl(string input)
    {
        Sprite content = null;
        int position = 0;

        if (input.Contains("Button2")) position = 0;
        else if (input.Contains("Button3")) position = 1;
        else if (input.Contains("Button0")) position = 2;
        else if (input.Contains("Button1")) position = 3;
        else if (input.Contains("Button4")) position = 4;
        else if (input.Contains("Button5")) position = 5;
        else if (input.Contains("Button6")) position = 6;
        else if (input.Contains("Button7")) position = 7;

        switch (_typeController)
        {
            case TypeController.XBox:
                content = xboxUI[position];
                break;
            case TypeController.Keyboard:
                content = keyboardMouseUI[position];
                break;
            case TypeController.PlayStation:
                content = playStationUI[position];
                break;
        }

        return content;
    }
}
