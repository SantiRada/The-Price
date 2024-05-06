using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    [Header("All Content")]
    [SerializeField] private GameObject[] _sectorSettings;
    [Space]
    [SerializeField] private Image[] _allContent;
    [SerializeField] private Selectable[] _allContentSelectable;
    [Space]
    [SerializeField] private Sprite _sprBase;
    [SerializeField] private Sprite _sprSelect;

    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI[] _textTitles;
    [SerializeField] private int[] _countForSection;
    private bool inConfig { get; set; }
    private int _indexConfig = 0;
    private int _posInSettings = 0;
    private int _prevPosInSettings = 0;

    [Header("Game")]
    [SerializeField] private TextMeshProUGUI _descriptionSection;

    [Header("Function for Dropdown")]
    [SerializeField] private int[] _gameDropdown;
    [SerializeField] private int[] _screenDropdown;
    private float _timerToDelayRun = 0.1f;
    private bool _canRun = true;

    [Header("Movement UI")]
    [SerializeField] private float _delayToMovementStick = 1f;
    private bool _canMovement = true;

    [Header("Calls")]
    private ControlSettings _controlSettings;
    private LanguageManager _languageManager;

    private void Awake()
    {
        _controlSettings = GetComponent<ControlSettings>();
        _languageManager = FindAnyObjectByType<LanguageManager>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        Config = false;

        _indexConfig = 0;
        _posInSettings = 0;

        for (int i = 0; i < _textTitles.Length; i++)
        {
            _textTitles[i].color = Color.gray;
        }
        _textTitles[0].color = Color.white;

        for (int i = 1; i < _sectorSettings.Length; i++)
        {
            _sectorSettings[i].SetActive(false);
        }
    }
    private void Update()
    {
        // MOVER HACIA ATRAS
        if (Input.GetButtonDown("LB") || Input.GetKeyDown(KeyCode.Q))
        {
            int index = _indexConfig;

            if (index == 0) index = 2;
            else index--;

            MoveToSectionInSettings(index);
        }

        // MOVER HACIA ADELANTE
        if (Input.GetButtonDown("RB") || Input.GetKeyDown(KeyCode.E))
        {
            int index = _indexConfig;

            if (index >= 2) index = 0;
            else index++;

            MoveToSectionInSettings(index);
        }

        // MOVIMIENTO INTERNO EN CONFIGURATION
        if (Input.GetAxis("Vertical") != 0 && _canMovement && _canRun)
        {
            StartCoroutine(MovementSection(Input.GetAxis("Vertical")));
        }

        // CANCELAR MOVIMIENTO AL ABRIR UN DROPDOWN
        if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3") || Input.GetButtonDown("Submit") || Input.GetButtonDown("Select")) && Config)
        {
            switch (_indexConfig)
            {
                case 0:
                    for (int i = 0; i < _gameDropdown.Length; i++)
                    {
                        if (_posInSettings == _gameDropdown[i])
                        {
                            _canRun = false;
                            break;
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < _screenDropdown.Length; i++)
                    {
                        if (_posInSettings == _screenDropdown[i])
                        {
                            _canRun = false;
                            break;
                        }
                    }
                    break;
            }
        }
        if (!_canRun)
        {
            _timerToDelayRun -= Time.deltaTime;

            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3") || Input.GetButtonDown("Submit") || Input.GetButtonDown("Select") || Input.GetButtonDown("Cancel") || Input.GetButtonDown("Jump"))
            {
                if (_timerToDelayRun <= 0)
                {
                    _canRun = true;
                    _timerToDelayRun = 0.1f;
                }
            }
        }
    }
    // ---- SETTER & GETTER ---- //
    public bool Config
    {
        get { return inConfig; }
        set { inConfig = value; }
    }
    // ---- MANIPULATE ---- //
    public void OpenConfig()
    {
        Config = true;
        _indexConfig = 0;
        _posInSettings = 0;
        _prevPosInSettings = 0;

        _sectorSettings[0].SetActive(true);

        _allContentSelectable[0].Select();
    }
    public void CloseConfig()
    {
        _controlSettings.SaveData();

        Config = false;

        _sectorSettings[_indexConfig].SetActive(false);
    }
    // ---------- MOVEMENT ---------- //
    private IEnumerator MovementSection(float num)
    {
        _canMovement = false;

        // PINTAR DE NEGRO EL ELEMENTO ANTERIOR
        if (_indexConfig > 0)
        {
            RestartPrevSelect();

            _allContent[_prevPosInSettings].sprite = _sprBase;
        }
        else
        {
            _allContent[_posInSettings].sprite = _sprBase;
        }
        // --------------------------------- //

        // -- MODIFICAR VALOR DE CONFIG Y MOVER A OTRO ELEMENTO
        if (num > 0)
        {
            _prevPosInSettings--;
            _posInSettings--;
        }
        if (num < 0)
        {
            _prevPosInSettings++;
            _posInSettings++;
        }

        // CALCULAR POSICIÓN MÁXIMA SEGÚN LA SECCIÓN //
        if (_posInSettings < 0)
        {
            _posInSettings = _countForSection[_indexConfig] - 1;
        }
        if (_posInSettings >= _countForSection[_indexConfig])
        {
            _posInSettings = 0;
            RestartPrevSelect();
        }
        // ----------------------------------------- //

        // COLOCAR TEXTO DESCRIPTIVO SI VIENE AL CASO //
        if (_indexConfig == 0)
        {
            int _space = _posInSettings + 49;

            _descriptionSection.text = _languageManager.GetValue(_space);
        }
        else { _descriptionSection.text = ""; }
        // ------------------------------------------ //

        // PINTAR DE GRIS EL NUEVO ELEMENTO
        if (_indexConfig > 0)
        {
            _allContent[_prevPosInSettings].sprite = _sprSelect;
            _allContentSelectable[_prevPosInSettings].Select();
        }
        else
        {
            _allContent[_posInSettings].sprite = _sprSelect;
            _allContentSelectable[_posInSettings].Select();
        }

        yield return new WaitForSeconds(_delayToMovementStick);
        _canMovement = true;
    }
    // ---------- SETTINGS ---------- //
    public void MoveToSectionInSettings(int num)
    {
        if (_indexConfig > 0)
        {
            RestartPrevSelect();

            _allContent[_prevPosInSettings].sprite = _sprBase;
        }
        else
        {
            _allContent[_posInSettings].sprite = _sprBase;
        }

        _posInSettings = 0;

        _textTitles[_indexConfig].color = Color.gray;
        _sectorSettings[_indexConfig].SetActive(false);

        _indexConfig = num;

        _textTitles[_indexConfig].color = Color.white;
        _sectorSettings[_indexConfig].SetActive(true);

        if (_indexConfig > 0)
        {
            RestartPrevSelect();

            _allContent[_prevPosInSettings].sprite = _sprSelect;
        }
        else
        {
            _allContent[_posInSettings].sprite = _sprSelect;
        }
    }
    private void RestartPrevSelect()
    {
        _prevPosInSettings = _posInSettings;
        for (int i = 0; i < _indexConfig; i++)
        {
            _prevPosInSettings += _countForSection[i];
        }
    }
}
