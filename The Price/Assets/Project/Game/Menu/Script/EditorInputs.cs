using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EditorInputs : MonoBehaviour {

    [Header("Reference Controls")]
    public InputActionReference[] _inputActionReference;
    public List<string> inputData = new List<string>();
    private string _scheme = "Gamepad";

    [Header("Confirmation")]
    [SerializeField] private GameObject _sectionConfirm;
    [SerializeField] private TextMeshProUGUI _textConfirm;
    [SerializeField] private float _timer;
    private float _baseTimer;
    private bool _inConfirm { get; set; }
    private int _positionChange;
    private string _textContent;

    [Header("Other Data")]
    private PlayerInput _inputAction;
    private Settings _settings;
    private DetectControlForUI _detector;

    private void Awake()
    {
        _inputAction = GetComponent<PlayerInput>();
        _settings = FindAnyObjectByType<Settings>();
        _detector = GetComponent<DetectControlForUI>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _baseTimer = _timer;
        _sectionConfirm.SetActive(false);

        DetectScheme();
    }
    private void DetectScheme()
    {
        _inputAction.SwitchCurrentControlScheme(_scheme);

        for (int i = 0; i < _inputActionReference.Length; i++)
        {
            for (int j = 0; j < _inputActionReference[i].action.bindings.Count; j++)
            {
                if (_inputActionReference[i].action.bindings[j].path.Contains(_scheme))
                {
                    if (inputData.Count != _inputActionReference.Length)
                        inputData.Add(_inputActionReference[i].action.bindings[j].path);
                    else
                        inputData[i] = (_inputActionReference[i].action.bindings[j].path);
                    // --------------------- //
                    break;
                }
            }
        }
    }
    private void Update()
    {
        if (InConfirm)
        {
            _timer -= Time.deltaTime;

            _textConfirm.text = _textContent + "\n\n" + _timer.ToString("f0");

            if (_timer < (_baseTimer - 0.5f))
            {
                if (Input.anyKeyDown)
                {
                    // DETECTAR TECLA O BOTON
                    string controlName = "";
                    foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKeyDown(keyCode))
                        {
                            controlName = keyCode.ToString();
                            break;
                        }
                    }

                    // CAMBIAR BINDING
                    ChangeBinding(controlName);
                }
            }

            if (_timer < 1) CloseConfirm();
        }
    }
    private void ChangeBinding(string controlName)
    {
        _settings.EditInterable();

        string _cleanKey = "<" + _inputAction.currentControlScheme + ">/";

        if (_inputAction.currentControlScheme == "Gamepad")
        {
            if (controlName.Contains("Button0"))
            {
                _cleanKey += "buttonSouth";
            }
            if (controlName.Contains("Button1"))
            {
                _cleanKey += "buttonEast";
            }
            if (controlName.Contains("Button2"))
            {
                _cleanKey += "buttonWest";
            }
            if (controlName.Contains("Button3"))
            {
                _cleanKey += "buttonNorth";
            }
            if (controlName.Contains("Button4"))
            {
                _cleanKey += "leftShoulder";
            }
            if (controlName.Contains("Button5"))
            {
                _cleanKey += "rightShoulder";
            }
            if (controlName.Contains("Button6"))
            {
                _cleanKey += "select";
            }
            if (controlName.Contains("Button7"))
            {
                _cleanKey += "start";
            }
            if (controlName.Contains("Button8"))
            {
                _cleanKey += "leftStickPress";
            }
            if (controlName.Contains("Button9"))
            {
                _cleanKey += "rightStickPress";
            }
            if (controlName.Contains("Button10"))
            {
                _cleanKey += "leftTrigger";
            }
            if (controlName.Contains("Button11"))
            {
                _cleanKey += "rightTrigger";
            }
        }
        else
        {
            if (!controlName.Contains("Joystick")) _cleanKey += controlName;
        }

        _inputActionReference[_positionChange].action.ChangeBinding(_cleanKey);

        inputData[_positionChange] = _cleanKey;

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
        _scheme = "Gamepad";
        DetectScheme();

        Edit(position);
    }
    public void EditKeyboard(int position)
    {
        _scheme = "Keyboard";
        DetectScheme();

        Edit(position);
    }
    private void CloseConfirm()
    {
        _positionChange = -1;

        InConfirm = false;
        _textConfirm.text = _textContent;

        _settings.Invoke("EditInterable", 0.5f);

        _detector.ChangeDetectValues();

        _sectionConfirm.SetActive(InConfirm);
    }
    // ----- SETTER & GETTER ----- //
    public bool InConfirm
    {
        get { return _inConfirm; }
        set { _inConfirm = value; }
    }
}