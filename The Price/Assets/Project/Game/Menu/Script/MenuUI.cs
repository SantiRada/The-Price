using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour {

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

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _detectorPlayers = FindAnyObjectByType<DetectorPlayers>();
        _language = FindAnyObjectByType<LanguageManager>();
        _controlSettings = GetComponentInChildren<Settings>();
    }
    private void Start()
    {
        InitialValues();
    }
    private void InitialValues()
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

    }
    private void Update()
    {
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

        if (_posGeneral == 0)
        {
            _FirstElement.Select();
        }
        else if (_posGeneral == 1)
        {
            _controlSettings.OpenConfig();
        }

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
}
