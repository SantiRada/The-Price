using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour {

    [SerializeField] private static string _versionOfGame = "0.0.1v";

    [Header("General Section")]
    [SerializeField] private GameObject[] _sectioners;
    [SerializeField] private Selectable _FirstElement;
    private int _posGeneral;

    [Header("Credits")]
    [SerializeField] private TextMeshProUGUI _creditText;
    private bool _inCredit = false;
    private bool _canChangeVelocity = false;

    [Header("Private Data")]
    private Animator _anim;
    private LanguageManager _language;
    private DetectorPlayers _detectorPlayers;
    private Settings _controlSettings;
    private EditorInputs _editorInputs;
    private InputManager _inputManager;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _editorInputs = FindAnyObjectByType<EditorInputs>();
        _detectorPlayers = FindAnyObjectByType<DetectorPlayers>();
        _language = FindAnyObjectByType<LanguageManager>();
        _controlSettings = GetComponentInChildren<Settings>();
        _inputManager = FindAnyObjectByType<InputManager>();
    }
    private void Start()
    {
        InitialValues();
    }
    public void InitialValues()
    {
        _posGeneral = 0;

        _FirstElement.Select();

        _creditText.text += _language.GetValue(58) + "\n";
        _creditText.text += _language.GetValue(59) + "\n\n";
        _creditText.text += _language.GetValue(60) + "\n";
        _creditText.text += _language.GetValue(61);

        for(int i = 1; i < _sectioners.Length; i++)
        {
            _sectioners[i].SetActive(false);
        }

        LoadingScreen.CountElement++;
    }
    private void Update()
    {
        if (_editorInputs.InConfirm || !_editorInputs.canUseThatKey || LoadingScreen.InLoading) return;

        // CERRAR SETTINGS Y VOLVER AL MENU BASE
        if ((Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape)) && _posGeneral == 1)
        {
            _controlSettings.CloseConfig();

            MoveToSection(0);
        }

        // CERRAR CRÉDITOS MANUALMENTE 
        if (Input.GetButtonDown("Fire3") && _inCredit) CloseCredits();

        // MANIPULAR VELOCIDAD DEL ANIMATOR
        if (_inCredit && _canChangeVelocity)
        {
            if (Input.GetButton("Fire1")) _anim.speed = 4;
            if (Input.GetButtonUp("Fire1")) _anim.speed = 1;
        }

        DetectControl();
    }
    private void DetectControl()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    string controlName = keyCode.ToString();

                    List<TypeController> listType = new List<TypeController>();

                    if (controlName.ToLower().Contains("joystick"))
                    {
                        int value = 0;
                        if (!controlName.ToLower().Contains("kb"))
                        {
                            // Es un número de joystick mayor a 0
                            string[] data = controlName.Split('k');
                            string[] subdata = data[1].Split('B');

                            value = int.Parse(subdata[0]);
                        }


                        if (Input.GetJoystickNames()[value].ToLower().Contains("xbox")) listType.Add(TypeController.Xbox);
                        else listType.Add(TypeController.PlayStation);
                    }
                    else { listType.Add(TypeController.Keyboard); }

                    _inputManager.SetTypeControllers(listType);
                    _inputManager.ChangeDetectValues();
                    break;
                }
            }
        }
    }
    public void CloseCredits()
    {
        _inCredit = false;
        MoveToSection(0);
    }
    // ---------- GENERAL ---------- //
    public void MoveToSection(int num)
    {
        _sectioners[_posGeneral].SetActive(false);
        _posGeneral = num;
        _sectioners[_posGeneral].SetActive(true);

        if (_posGeneral == 0) _FirstElement.Select();
        else if (_posGeneral == 1) _controlSettings.OpenConfig();

        if (num == 2)
        {
            _inCredit = true;

            Invoke("CanChangeSpeedCredits", 0.5f);
        }
        else
        {
            _inCredit = false;
        }

        if(num == 3) _detectorPlayers.canDetect = true;
        else _detectorPlayers.canDetect = false;

        _anim.SetInteger("Screen", _posGeneral);
        _anim.SetBool("Credits", _inCredit);
    }
    private void CanChangeSpeedCredits()
    {
        _canChangeVelocity = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public static string GetVersion()
    {
        return _versionOfGame;
    }
}
