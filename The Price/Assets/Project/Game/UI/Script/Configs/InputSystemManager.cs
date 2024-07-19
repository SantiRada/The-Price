using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum TypeController { Null, Xbox, PlayStation, Keyboard }
public class InputSystemManager : MonoBehaviour {

    [Header("Content UI")]
    public Sprite[] xboxInput;
    public Sprite[] psInput;
    public Sprite[] keyboardInput;

    [Header("Private Content")]
    private Image[] keys;
    private TextMeshProUGUI[] textKeys;
    public TypeController _typeController = TypeController.Null;
    private SaveLoadManager _saveLoad;
    private SaveControls _controls;

    private TypeController _oldTypeController = TypeController.Null;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _saveLoad = FindAnyObjectByType<SaveLoadManager>();
        _playerInput = FindAnyObjectByType<PlayerInput>();

        SeparateKeys();
    }
    private void Start() { Invoke("GetControls", 0.25f); }
    private void GetControls()
    {
        if(_saveLoad.GetControls() != null) { _controls = _saveLoad.GetControls(); }
        else { _controls = new SaveControls(); }
    }
    private void Update()
    {
        if (LoadingScreen.inLoading) return;

        VerifyControl();
    }
    private void SeparateKeys()
    {
        // ALL IMAGES
        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        List<Image> allImageWithNameKeys = new List<Image>();

        foreach (Image imageElement in allImages)
        {
            if (imageElement.name.Contains("Keys")) allImageWithNameKeys.Add(imageElement);
        }

        keys = allImageWithNameKeys.ToArray();

        // TEXT MESH PRO U-GUI
        List<TextMeshProUGUI> allText = new List<TextMeshProUGUI>();

        for(int i = 0; i< keys.Length; i++) { allText.Add(keys[i].GetComponentInChildren<TextMeshProUGUI>()); }

        textKeys = allText.ToArray();

        for (int i = 0; i < textKeys.Length; i++) { textKeys[i].text = ""; }
    }
    public void ApplyChanges()
    {
        _oldTypeController = _typeController;

        for (int i = 0; i < keys.Length; i++)
        {
            string[] dataText = keys[i].name.Split('['); // dataText[0] = "Keys[" // dataText[1] = "Use]"
            string[] dataValues = dataText[1].Split(']'); // dataValues[0] = "Use" // dataValues[1] = "]"

            switch (dataValues[0])
            {
                case "MoveUp":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.moveUp]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.moveUp]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "MoveDown":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.moveDown]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.moveDown]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "MoveLeft":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.moveLeft]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.moveLeft]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "MoveRight":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.moveRight]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.moveRight]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "Use":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.use]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.use]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "Dash":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.dash]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.dash]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "StaticAim":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.staticAim]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.staticAim]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "AimUp":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.aimUp]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.aimUp]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "AimDown":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.aimDown]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.aimDown]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "AimLeft":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.aimLeft]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.aimLeft]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "AimRight":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.aimRight]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.aimRight]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "Stats":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.stats]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.stats]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "Pause":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.pause]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.pause]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "AttackOne":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.attackOne]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.attackOne]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "AttackTwo":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.attackTwo]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.attackTwo]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "AttackThree":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.attackThree]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.attackThree]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "SkillOne":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.skillOne]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.skillOne]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "SkillTwo":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.skillTwo]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.skillTwo]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "SkillThree":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.skillThree]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.skillThree]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "Back":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.back]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.back]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "Select":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.select]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.select]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "ResetValues":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.resetValues]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.resetValues]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "OtherFunction":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.otherFunction]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.otherFunction]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;

                case "LeftUI":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.leftUI]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.leftUI]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
                case "RightUI":
                    if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.rightUI]; }
                    else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.rightUI]; }
                    else // KEYBOARD
                    {
                        keys[i].sprite = keyboardInput[0];
                        textKeys[i].text = "";
                    }
                    break;
            }
        }
    }
    private void VerifyControl()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key.ToString().ToLower().Contains("joystick"))
                    {
                        key.ToString().ToLower();
                        string[] separateName = key.ToString().Split('k'); // [0] = joystick // [1] = 0button1
                        string[] separateValues = separateName[1].Split('b'); // [0] = "0" // [1] = button1

                        string[] nameJoysticks = Input.GetJoystickNames();

                        // VERIFICAR POSICIÓN DEL JOYSTICK
                        string value = separateValues[0];
                        int number;
                        bool isNumber = int.TryParse(value, out number);
                        if (!isNumber) number = 0;

                        if (nameJoysticks[number].ToLower().Contains("xbox"))
                        {
                            Debug.Log("Es un Joystick de Xbox");
                            _typeController = TypeController.Xbox;
                        }
                        else if (nameJoysticks[int.Parse(separateValues[0])].ToLower().Contains("play") || nameJoysticks[int.Parse(separateValues[0])].ToLower().Contains("dualshock"))
                        {
                            Debug.Log("Es un Joystick de Play Station");
                            _typeController = TypeController.PlayStation;
                        }
                    }
                    else
                    {
                        Debug.Log("Se presionó un teclado");
                        _typeController = TypeController.Keyboard;
                    }

                    if (_typeController != _oldTypeController) ApplyChanges();
                }
            }
        }
    }
}
