using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum TypeController { Null, Xbox, PlayStation, Keyboard }
public class InputSystemManager : MonoBehaviour {

    [Header("Content UI")]
    public float timeOfRecolor;
    public Color recolor;
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
        if (_saveLoad == null) return;

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
        if (_oldTypeController != TypeController.Keyboard) { for(int i = 0; i < textKeys.Length; i++) { textKeys[i].text = ""; } }

        for (int i = 0; i < keys.Length; i++)
        {
            string[] dataText = keys[i].name.Split('['); // dataText[0] = "Keys[" // dataText[1] = "1-Use]"
            string[] dataValues = dataText[1].Split(']'); // dataValues[0] = "1-Use" // dataValues[1] = "]"
            string[] dataFinal = dataValues[0].Split('-'); // dataFinal[0] = "1" // dataFinal[1] = "Use"

            if (dataFinal[0] == "1")
            {
                if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.GetValueControl(dataFinal[1])]; }
                else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.GetValueControl(dataFinal[1])]; }
                else { SetChangeKeyboard(i, dataFinal[1], "Player"); }
            }
            else
            {
                if (_oldTypeController == TypeController.Xbox) { keys[i].sprite = xboxInput[_controls.GetValueControl(dataFinal[1])]; }
                else if (_oldTypeController == TypeController.PlayStation) { keys[i].sprite = psInput[_controls.GetValueControl(dataFinal[1])]; }
                else { SetChangeKeyboard(i, dataFinal[1], "Pause"); }
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

                        if (nameJoysticks[number].ToLower().Contains("station") || nameJoysticks[number].ToLower().Contains("wireless"))
                        {
                            // Debug.Log("Es un Joystick de Play Station");
                            _typeController = TypeController.PlayStation;
                        }
                        else if (nameJoysticks[number].ToLower().Contains("xbox"))
                        {
                            // Debug.Log("Es un Joystick de Xbox");
                            _typeController = TypeController.Xbox;
                        }
                        else
                        {
                            // Debug.Log("Es un Joystick Genérico");
                            _typeController = TypeController.Xbox;
                        }
                    }
                    else { _typeController = TypeController.Keyboard; }

                    if (_typeController != _oldTypeController) ApplyChanges();
                }
            }
        }
    }
    // ---- FUNCION INTEGRA ---- //
    private Sprite GetSpriteKeyboard(string binding)
    {
        Sprite spr = keyboardInput[0];

        if(binding.Length > 1)
        {
            if (binding.ToLower().Contains("arrow"))
            {
                switch (binding.ToLower())
                {
                    case "uparrow": spr = keyboardInput[4]; break;
                    case "downarrow": spr = keyboardInput[5]; break;
                    case "leftarrow": spr = keyboardInput[6]; break;
                    case "rightarrow": spr = keyboardInput[7]; break;
                }
            }
            else if (binding.ToLower().Contains("shift")) { spr = keyboardInput[2]; }
            else if (binding.ToLower().Contains("back")) { spr = keyboardInput[7]; }
            else { spr = keyboardInput[1]; }
        }

        return spr;
    }
    private string GetKeyboardName(string data, string actionMap = "Player")
    {
        string dataSearch = data;
        if (data.Contains("Move")) dataSearch = "Move";
        else if (data.Contains("Aim")) dataSearch = "Aim";

        var actionMapping = _playerInput.actions.FindActionMap(actionMap);
        var action = actionMapping.FindAction(dataSearch);

        string binding = "";
        int iteration = 0;

        foreach (var values in action.bindings)
        {
            if (values.groups.Contains("Keyboard"))
            {
                string[] separateName = values.ToString().Split("/");
                string[] deleteFinish = separateName[1].Split("[");

                if (data.Contains("Move"))
                {
                    if (data == "MoveUp" && iteration == 0) binding = deleteFinish[0].ToUpper();
                    else if (data == "MoveDown" && iteration == 1) binding = deleteFinish[0].ToUpper();
                    else if (data == "MoveLeft" && iteration == 2) binding = deleteFinish[0].ToUpper();
                    else if (data == "MoveRight" && iteration == 3) binding = deleteFinish[0].ToUpper();

                    iteration++;
                }
                else if (data.Contains("Aim"))
                {
                    if (data == "AimUp" && iteration == 0) binding = deleteFinish[0].ToUpper();
                    else if (data == "AimDown" && iteration == 1) binding = deleteFinish[0].ToUpper();
                    else if (data == "AimLeft" && iteration == 2) binding = deleteFinish[0].ToUpper();
                    else if (data == "AimRight" && iteration == 3) binding = deleteFinish[0].ToUpper();

                    iteration++;
                }
                else { binding = deleteFinish[0].ToUpper(); }
                
                if (binding != "" && binding != null) break;
            }
        }

        // RENAME SPECIFIC KEYS
        switch (binding)
        {
            case "BACKSPACE": binding = "BACK"; break;
            case "CONTROL": binding = "CTRL"; break;
            case "LEFTCONTROL": binding = "CTRL"; break;
            case "RIGHTCONTROL": binding = "CTRL"; break;
            case "LEFTALT": binding = "ALT"; break;
            case "RIGHTALT": binding = "ALT"; break;
            case "DELETE": binding = "SUPR"; break;
            case "INSERT": binding = "INS"; break;
            case "PAGEUP": binding = "REPAG"; break;
            case "PAGEDOWN": binding = "AVPAG"; break;
            case "OEM1": binding = "<"; break;
            case "OEM2": binding = ">"; break;
            case "ENTER": binding = "RETURN"; break;
            case "NUMPADENTER": binding = "INTRO"; break;
        }

        return binding;
    }
    private void SetChangeKeyboard(int pos, string name, string actionMap)
    {
        string binding = GetKeyboardName(name, actionMap);
        keys[pos].sprite = GetSpriteKeyboard(binding);

        if (!binding.ToLower().Contains("arrow") && !binding.ToLower().Contains("shift") && !binding.ToLower().Contains("back")) textKeys[pos].text = binding;
    }
    // ---- ANIMATION ---- //
    public void ApplyAnimation(string use)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].gameObject.activeSelf)
            {
                string[] separateName = keys[i].name.Split('-'); // [0] = "Keys[1-" // [1] = Use]
                string[] dataFinal = separateName[1].Split(']'); // [0] = "Use" // [1] = "]"

                if (use == dataFinal[0])
                {
                    StartCoroutine(AnimationKey(keys[i]));
                }
            }
        }
    }
    private IEnumerator AnimationKey(Image img)
    {
        img.color = recolor;

        yield return new WaitForSeconds(timeOfRecolor);

        img.color = Color.white;
    }
}
