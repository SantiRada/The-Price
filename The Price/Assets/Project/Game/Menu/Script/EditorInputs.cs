using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EditorInputs : MonoBehaviour {

    [Header("Reference Controls")]
    [SerializeField] private InputActionReference[] _inputActionReference;
    [SerializeField] private List<string> _inputData = new List<string>();

    [Header("Other Data")]
    private PlayerInput _inputAction;
    private Settings _settings;

    [Header("Confirmation")]
    [SerializeField] private GameObject _sectionConfirm;
    [SerializeField] private TextMeshProUGUI _textConfirm;
    [SerializeField] private float _timer;
    private float _baseTimer;
    private bool _inConfirm { get; set; }
    private int _positionChange;
    private string _textContent;

    private void Awake()
    {
        _inputAction = GetComponent<PlayerInput>();
        _settings = GetComponentInParent<Settings>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
    {
        _baseTimer = _timer;
        _sectionConfirm.SetActive(false);

        _inputAction.SwitchCurrentControlScheme("Gamepad");

        for (int i = 0; i < _inputActionReference.Length; i++)
        {
            _inputData.Add(_inputActionReference[i].action.bindings[0].path);
        }
    }
    private void Update()
    {
        if (InConfirm)
        {
            _timer -= Time.deltaTime;

            _textConfirm.text = _textContent + "\n\n" + _timer.ToString("f0");

            if(_timer < (_baseTimer - 0.5f))
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

            if (_timer < 0)
            {
                CloseConfirm();
            }
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

        _inputData[_positionChange] = _cleanKey;

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
        Debug.Log("Si");

        _inputAction.SwitchCurrentControlScheme("Gamepad");

        Edit(position);
    }
    public void EditKeyboard(int position)
    {
        Debug.Log("Si");

        _inputAction.SwitchCurrentControlScheme("Keyboard");

        Edit(position);
    }
    private void CloseConfirm()
    {
        _positionChange = -1;

        InConfirm = false;
        _textConfirm.text = _textContent;

        _settings.Invoke("EditInterable", 0.5f);

        _sectionConfirm.SetActive(InConfirm);
    }
    // ----- SETTER & GETTER ----- //
    public bool InConfirm
    {
        get { return _inConfirm; }
        set { _inConfirm = value; }
    }
}