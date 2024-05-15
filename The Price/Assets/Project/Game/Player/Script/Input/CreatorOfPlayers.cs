using System.Collections.Generic;
using UnityEngine;

public enum TypeController { Keyboard, Xbox, PlayStation }
public class CreatorOfPlayers : MonoBehaviour {

    [Header("Detector Players")]
    public List<string> _players = new List<string>();

    [Header("Content Game")]
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _hudPlayer;
    [SerializeField] private GameObject _statsPlayer;
    [SerializeField] private List<TypeController> _typeController = new List<TypeController>();

    private InputManager _inputManager;

    private void Awake()
    {
        _inputManager = FindAnyObjectByType<InputManager>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        string[] data = PlayerPrefs.GetString("dataPlayers", "Keyboard & Mouse").Split(',');

        for (int i = 0; i < PlayerPrefs.GetInt("countPlayers", 1); i++)
        {
            _players.Add(data[i]);
        }

        CreatePlayersInScene();

        LoadingScreen.CountElement++;
    }
    private void CreatePlayersInScene()
    {
        Transform parentHUD = GameObject.FindGameObjectWithTag("HUD").transform;
        Transform parentStats = GameObject.FindGameObjectWithTag("UI").transform;

        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].Contains("Keyboard"))
            {
                _typeController.Add(TypeController.Keyboard);
            }
            else
            {
                string[] data = _players[i].Split('k');
                int lastCharacter = int.Parse(data[1]) - 1;
                if (Input.GetJoystickNames()[lastCharacter].ToLower().Contains("xbox")) _typeController.Add(TypeController.Xbox);
                else _typeController.Add(TypeController.PlayStation);
            }

            // ---- PLAYER -------------------- //
            GameObject obj = Instantiate(_playerObj, new Vector3(i, 0, 0), Quaternion.identity);
            PlayerStats pj = obj.GetComponent<PlayerStats>();
            pj.Control = _players[i];

            // ---- HUD ----------------------- //
            GameObject objHUD = Instantiate(_hudPlayer, Vector3.zero, Quaternion.identity, parentHUD);
            objHUD.transform.position = Vector3.zero;
            objHUD.tag = "player" + (i+1).ToString();
            
            // ---- STATS --------------------- //

            GameObject objStats = Instantiate(_statsPlayer, Vector3.zero, Quaternion.identity, parentStats);
            objStats.tag = "player" + (i + 1).ToString();
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

            ApplyAllTags(objHUD, objStats, i);

            pj.SetUI(objHUD, objStats, _players[i]);
        }

        _inputManager.SetTypeControllers(_typeController);
    }
    private void ApplyAllTags(GameObject hud, GameObject stats, int i)
    {
        Transform[] childrenHud = hud.GetComponentsInChildren<Transform>(true);
        Transform[] childrenStats = stats.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childrenHud)
        {
            child.gameObject.tag = "player" + (i + 1).ToString();
        }
        foreach (Transform child in childrenStats)
        {
            child.gameObject.tag = "player" + (i + 1).ToString();
        }
    }
}
