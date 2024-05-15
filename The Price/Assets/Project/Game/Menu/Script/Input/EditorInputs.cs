using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class EditorInputs : MonoBehaviour {

    [Header("Reference Controls")]
    public InputActionReference[] _inputActionReference;
    private string _schemeModifier = "Gamepad";
    private int _positionChange;

    [Header("Confirmation")]
    [SerializeField] private GameObject _sectionConfirm;
    [SerializeField] private TextMeshProUGUI _textConfirm;
    [SerializeField] private float _timer;
    private float _baseTimer;
    private bool _inConfirm { get; set; }

    public bool canUseThatKey = true;

    [Header("Data UI")]
    [SerializeField] private Image[] _contentKeyboardUI;
    [SerializeField] private Image[] _contentGamepadUI;

    [Header("Other Data")]
    private Settings _settings;
    private InputManager _inputManager;
    private LanguageManager _language;

    private void Awake()
    {
        _settings = FindAnyObjectByType<Settings>();
        _inputManager = GetComponent<InputManager>();
        _language = GetComponent<LanguageManager>();
    }
    private void OnEnable()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _baseTimer = _timer;
        _sectionConfirm.SetActive(false);

        LoadingScreen.CountElement++;
    }
    private void Update()
    {
        if (InConfirm)
        {
            _timer -= Time.deltaTime;

            _textConfirm.text = _language.GetValue(43) + "\n\n" + _timer.ToString("f0");

            if (Input.anyKeyDown)
            {
                canUseThatKey = false;
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        string controlName = keyCode.ToString();
                        ChangeBinding(controlName);
                        break;
                    }
                }
            }

            if (_timer < 1) CloseConfirm();
        }
        else canUseThatKey = true;
    }
    private void ChangeBinding(string controlName)
    {
        string _cleanKey = "<" + _schemeModifier + ">/";
        int positionPad = 0;

        if (_schemeModifier == "Gamepad")
        {
            if (Regex.IsMatch(controlName, @"Button0\b"))
            {
                _cleanKey += "buttonSouth";
                positionPad = 2;
            }
            if (Regex.IsMatch(controlName, @"Button1\b"))
            {
                _cleanKey += "buttonEast";
                positionPad = 5;
            }
            if (Regex.IsMatch(controlName, @"Button2\b"))
            {
                _cleanKey += "buttonWest";
                positionPad = 1;
            }
            if (Regex.IsMatch(controlName, @"Button3\b"))
            {
                _cleanKey += "buttonNorth";
                positionPad = 4;
            }
            if (Regex.IsMatch(controlName, @"Button4\b"))
            {
                _cleanKey += "leftShoulder";
                positionPad = 3;
            }
            if (Regex.IsMatch(controlName, @"Button5\b"))
            {
                _cleanKey += "rightShoulder";
                positionPad = 0;
            }
            if (Regex.IsMatch(controlName, @"Button6\b"))
            {
                _cleanKey += "select";
                positionPad = 6;
            }
            if (Regex.IsMatch(controlName, @"Button7\b"))
            {
                _cleanKey += "start";
                positionPad = 7;
            }
            if (Regex.IsMatch(controlName, @"Button8\b"))
            {
                _cleanKey += "leftStickPress";
                positionPad = 10;
            }
            if (Regex.IsMatch(controlName, @"Button9\b"))
            {
                _cleanKey += "rightStickPress";
                positionPad = 11;
            }
        }
        else { if (!controlName.Contains("Joystick")) _cleanKey += controlName; }

        // ---- CHANGE BINDING IN *InputAction* ---- //
        _inputActionReference[_positionChange].action.ChangeBinding(controlName);

        // ---- CHANGE BINDING IN *InputManager* --- //
        if (_schemeModifier == "Gamepad") _inputManager.ChangeGamepad(_inputActionReference[_positionChange].action.name, positionPad);
        else _inputManager.ChangeKeyboard(_positionChange, _cleanKey);

        CloseConfirm();
    }
    private void Edit(int position)
    {
        if (!canUseThatKey) return;

        _positionChange = position;

        _timer = _baseTimer;

        _settings.enabled = false;

        InConfirm = true;
        _sectionConfirm.SetActive(InConfirm);
    }
    public void EditGamepad(int position)
    {
        _schemeModifier = "Gamepad";

        Edit(position);
    }
    public void EditKeyboard(int position)
    {
        _schemeModifier = "Keyboard";

        Edit(position);
    }
    private void CloseConfirm()
    {
        _positionChange = -1;

        InConfirm = false;

        _settings.enabled = true;

        _inputManager.ChangeDetectValues(); 

        _sectionConfirm.SetActive(InConfirm);
    }
    // ----- SETTER & GETTER ----- //
    public bool InConfirm
    {
        get { return _inConfirm; }
        set { _inConfirm = value; }
    }
}