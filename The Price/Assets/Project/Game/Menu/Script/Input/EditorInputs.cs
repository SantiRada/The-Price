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
    private string _textContent;

    [Header("Data UI")]
    [SerializeField] private Image[] _contentKeyboardUI;
    [SerializeField] private Image[] _contentGamepadUI;

    [Header("Other Data")]
    private Settings _settings;
    private InputManager _inputManager;

    private void Awake()
    {
        _settings = FindAnyObjectByType<Settings>();
        _inputManager = GetComponent<InputManager>();
    }
    private void Start()
    {
        _baseTimer = _timer;
        _sectionConfirm.SetActive(false);
    }
    private void Update()
    {
        if (InConfirm)
        {
            _timer -= Time.deltaTime;

            _textConfirm.text = _textContent + "\n\n" + _timer.ToString("f0");


            if (Input.anyKeyDown)
            {
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
    }
    private void ChangeBinding(string controlName)
    {
        _settings.EditInterable();

        string _cleanKey = "<" + _schemeModifier + ">/";

        if (_schemeModifier == "Gamepad")
        {
            if (Regex.IsMatch(controlName, @"Button0\b")) _cleanKey += "buttonSouth";
            if (Regex.IsMatch(controlName, @"Button1\b")) _cleanKey += "buttonEast";
            if (Regex.IsMatch(controlName, @"Button2\b")) _cleanKey += "buttonWest";
            if (Regex.IsMatch(controlName, @"Button3\b")) _cleanKey += "buttonNorth";
            if (Regex.IsMatch(controlName, @"Button4\b")) _cleanKey += "leftShoulder";
            if (Regex.IsMatch(controlName, @"Button5\b")) _cleanKey += "rightShoulder";
            if (Regex.IsMatch(controlName, @"Button6\b")) _cleanKey += "select";
            if (Regex.IsMatch(controlName, @"Button7\b")) _cleanKey += "start";
            if (Regex.IsMatch(controlName, @"Button8\b")) _cleanKey += "leftStickPress";
            if (Regex.IsMatch(controlName, @"Button9\b")) _cleanKey += "rightStickPress";
            if (Regex.IsMatch(controlName, @"Button10\b")) _cleanKey += "leftTrigger";
            if (Regex.IsMatch(controlName, @"Button11\b")) _cleanKey += "rightTrigger";
        }
        else { if (!controlName.Contains("Joystick")) _cleanKey += controlName; }

        // ---- CHANGE BINDING IN *InputAction* ---- //
        _inputActionReference[_positionChange].action.ChangeBinding(controlName);

        // ---- CHANGE BINDING IN *InputManager* --- //
        if (_schemeModifier == "Gamepad") _inputManager.ChangeGamepad(_inputActionReference[_positionChange].action.name, _positionChange); ////////// REVISAR REVISAR REVISAR
        else _inputManager.ChangeKeyboard(_positionChange, _cleanKey);

        // ---- RESET ELEMENTS IN THE UI ---- //
        if(_schemeModifier == "Gamepad")
        {
            for (int i = 0; i < _contentGamepadUI.Length; i++)
            {
                _inputManager.ResetOneElement(_contentGamepadUI[i]);
            }
        }
        else
        {
            for(int i = 0; i < _contentKeyboardUI.Length; i++)
            {
                _inputManager.ResetOneElement(_contentKeyboardUI[i]);
            }
        }

        CloseConfirm();
    }
    private void Edit(int position)
    {
        _positionChange = position;

        _textContent = _textConfirm.text;
        _timer = _baseTimer;

        _settings.EditInterable();

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
        _textConfirm.text = _textContent;

        _settings.Invoke("EditInterable", 0.5f);

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