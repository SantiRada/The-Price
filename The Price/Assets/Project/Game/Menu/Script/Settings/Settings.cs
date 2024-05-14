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
    [SerializeField] private Color _unselectedColor;
    [SerializeField] private Color _selectedColor;

    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI[] _textTitles;
    [SerializeField] private int[] _countForSection;
    private int _indexConfig = 0;
    private int _posInSettings = 0;
    private int _prevPosInSettings = 0;
    private int _localPosition = 0;

    [Header("Game")]
    [SerializeField] private TextMeshProUGUI _descriptionSection;

    [Header("Function for Dropdown")]
    [SerializeField] private int[] _dropdowns = { 8, 9, 10 };
    private float _timerToDelayRun = 0.1f;
    private bool _canRun = true;

    [Header("Movement UI")]
    [SerializeField] private float _delayToMovementStick = 1f;
    private bool _canMovement = true;

    [Header("Calls")]
    private ControlSettings _controlSettings;
    private LanguageManager _languageManager;
    private EditorInputs _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<EditorInputs>();
        _controlSettings = GetComponentInParent<ControlSettings>();
        _languageManager = FindAnyObjectByType<LanguageManager>();
    }
    private void Start()
    {
        Invoke("InitialValues", 0.25f);
    }
    private void InitialValues()
    {
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
        if (_inputs.InConfirm || !_inputs.canUseThatKey) return;

        // MOVER HACIA ATRAS
        if (Input.GetButtonDown("LB") || Input.GetKeyDown(KeyCode.Q))
        {
            int index = _indexConfig;

            if (index == 0) index = (_countForSection.Length - 1);
            else index--;

            MoveToSectionInSettings(index);
        }

        // MOVER HACIA ADELANTE
        if (Input.GetButtonDown("RB") || Input.GetKeyDown(KeyCode.E))
        {
            int index = _indexConfig;

            if (index >= (_countForSection.Length - 1)) index = 0;
            else index++;

            MoveToSectionInSettings(index);
        }

        // MOVIMIENTO INTERNO EN CONFIGURATION
        if (Input.GetAxis("Vertical") != 0 && _canMovement && _canRun)
        {
            StartCoroutine(MovementSection(Input.GetAxis("Vertical")));
        }

        if(Input.GetAxis("Horizontal") != 0 && _canMovement && _canRun && _posInSettings > 15)
        {
            if(Input.GetAxis("Horizontal") < 0) if (_posInSettings > 15 && _posInSettings < 24) return;

            if (Input.GetAxis("Horizontal") > 0) if (_posInSettings > 23) return;

            StartCoroutine(MovementSection(0));
        }

        // CANCELAR MOVIMIENTO AL ABRIR UN DROPDOWN
        if (Input.GetButtonDown("Fire1"))
        {
            for (int i = 0; i < _dropdowns.Length; i++)
            {
                if (_posInSettings == _dropdowns[i])
                {
                    _canRun = false;
                    break;
                }
            }
        }
        if (!_canRun)
        {
            _timerToDelayRun -= Time.deltaTime;

            if (Input.GetButtonDown("Fire1"))
            {
                if (_timerToDelayRun <= 0)
                {
                    _canRun = true;
                    _timerToDelayRun = 0.1f;
                }
            }
        }
    }
    public void EditInterable()
    {
        for(int i = 0; i < _allContentSelectable.Length; i++)
        {
            _allContentSelectable[_posInSettings].interactable = !_allContentSelectable[_posInSettings].interactable;
        }
    }
    // ---- MANIPULATE ---- //
    public void OpenConfig()
    {
        _indexConfig = 0;
        _posInSettings = 0;
        _prevPosInSettings = 0;

        _sectorSettings[0].SetActive(true);

        _allContentSelectable[0].Select();
    }
    public void CloseConfig()
    {
        _controlSettings.SaveData();

        _sectorSettings[_indexConfig].SetActive(false);
    }
    // ---------- MOVEMENT ---------- //
    private IEnumerator MovementSection(float num)
    {
        _canMovement = false;

        // PINTAR DE NEGRO EL ELEMENTO ANTERIOR
        _allContent[_prevPosInSettings].color = _unselectedColor;

        // CALCULAR POSICIONES SEGÚN LA SECCIÓN //
        TestToRestart(num);

        // COLOCAR TEXTO DESCRIPTIVO SI VIENE AL CASO //
        if (_indexConfig == 0)
        {
            int _space = _posInSettings + 49;

            _descriptionSection.text = _languageManager.GetValue(_space);
        }
        else { _descriptionSection.text = ""; }
        // ------------------------------------------ //

        // PINTAR DE GRIS EL NUEVO ELEMENTO
        _allContent[_posInSettings].color = _selectedColor;
        _allContentSelectable[_posInSettings].Select();

        yield return new WaitForSeconds(_delayToMovementStick);
        _canMovement = true;
    }
    // ---------- SETTINGS ---------- //
    public void MoveToSectionInSettings(int num)
    {
        _allContent[_prevPosInSettings].color = _unselectedColor;

        _textTitles[_indexConfig].color = Color.gray;
        _sectorSettings[_indexConfig].SetActive(false);

        _indexConfig = num;
        ResetPosition();

        _textTitles[_indexConfig].color = Color.white;
        _sectorSettings[_indexConfig].SetActive(true);

        _allContent[_posInSettings].color = _selectedColor;
        _allContentSelectable[_posInSettings].Select();
    }
    private void ResetPosition()
    {
        _posInSettings = _localPosition = 0;

        if (_indexConfig > 0)
        {
            for (int i = 0; i < _indexConfig; i++)
            {
                _posInSettings += _countForSection[i];
            }
        }

        _prevPosInSettings = _posInSettings;
    }
    private void TestToRestart(float num)
    {
        // -- MODIFICAR VALOR DE CONFIG Y MOVER A OTRO ELEMENTO
        if (num > 0) _localPosition--;
        if (num < 0) _localPosition++;

        // OPCIONES DE MOVIMIENTO HORIZONTAL PARA LOS CONTROLES
        #region Horizontal
        if (num == 0)
        {
            if(_posInSettings > 23) _localPosition -= 8;
            else _localPosition += 8;
        }
        #endregion

        #region Vertical
        if(num != 0)
        {
            if (_posInSettings == 23 || _posInSettings == 31)
            {
                if (num < 0) _localPosition -= 8;
            }

            if(_posInSettings == 16 || _posInSettings == 24)
            {
                if (num > 0) _localPosition += 8;
            }
        }

        #endregion
        // -----------------------------------------------------

        if (_localPosition >= _countForSection[_indexConfig]) _localPosition = 0;
        else if (_localPosition < 0) _localPosition = _countForSection[_indexConfig] - 1;

        _posInSettings = _localPosition;

        if(_indexConfig > 0)
        {
            for (int i = 0; i < _indexConfig; i++)
            {
                _posInSettings += _countForSection[i];
            }
        }

        _prevPosInSettings = _posInSettings;
    }
}