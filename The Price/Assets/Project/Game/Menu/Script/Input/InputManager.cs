using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    [Header("Data for Controls")]
    [SerializeField] private Sprite _sprForKeyboard;
    [SerializeField] private Sprite[] _sprForXbox = new Sprite[12];
    [SerializeField] private Sprite[] _sprForPlayStation = new Sprite[12];

    [Header("Data for Inputs")]
    [SerializeField] private List<string> _keyboardInputs = new List<string>();
    [SerializeField] private List<string> _formatsToGamepad = new List<string>();
    [SerializeField] private List<string> _formatsToKeyboard = new List<string>();

    [Header("Private Data")]
    [SerializeField] private List<Image> _contentKey = new List<Image>();
    [SerializeField] private List<string> _contentValue = new List<string>();

    private void Start()
    {
        ChangeDetectValues();
    }
    private void LoadAllPads()
    {
        Image[] _allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        List<Image> _alListImages = new List<Image>();

        foreach (Image contentImage in _allImages)
        {
            if (contentImage.name.Contains("Key"))
            {
                _alListImages.Add(contentImage);

                string value = SeparateNameImage(contentImage);

                _contentValue.Add(value);
            }
        }

        _contentKey = _alListImages;
    }
    private string SeparateNameImage(Image img)
    {
        string[] data = img.name.Split("[");
        string[] subdata = data[1].Split("]");

        return subdata[0];
    }
    public void ChangeDetectValues()
    {
        LoadAllPads();

        for (int i = 0; i < _contentKey.Count; i++)
        {
            _contentKey[i].sprite = GetInput(_contentKey[i].tag, _contentValue[i]);

            if (_contentKey[i].sprite == _sprForKeyboard)
            {
                // KEYBOARD SPECIFIC DATA
                TextMeshProUGUI tmKey = _contentKey[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();

                ChangeTextForKeyboard(tmKey, _contentValue[i]);
            }
        }
    }
    private void ChangeTextForKeyboard(TextMeshProUGUI tmpro, string value)
    {
        for(int i = 0; i < _formatsToKeyboard.Count; i++)
        {
            if (_formatsToKeyboard[i] == value)
            {
                tmpro.text = _keyboardInputs[i].ToString();
                break;
            }
        }
    }
    public Sprite GetInput(string element, string use)
    {
        for (int i = 0; i < _formatsToGamepad.Count; i++)
        {
            if (string.Compare(_formatsToGamepad[i].ToLower(), use.ToLower()) == 0)
            {
                // ENCONTRÉ LA POSICION DEL PARAMETRO BUSCADO
                if (element.Contains("xbox")) return _sprForXbox[i];
                else if (element.Contains("board")) return _sprForKeyboard;
                else return _sprForPlayStation[i];
            }
        }

        return null;
    }
    public Sprite ResetOneElement(Image img)
    {
        string value = SeparateNameImage(img);

        return GetInput(img.tag, value);
    }
    // ---- CHANGE BINDING VISUAL ---- //
    public void ChangeGamepad(string action, int newPosition)
    {
        for(int i = 0; i < _formatsToGamepad.Count; i++)
        {
            if (action.Contains(_formatsToGamepad[i]))
            {
                string prevValue = _formatsToGamepad[i];

                _formatsToGamepad[i] = _formatsToGamepad[newPosition];
                _formatsToGamepad[newPosition] = prevValue;
                break;
            }
        }
    }
    public void ChangeKeyboard(int position, string newInput)
    {
        _keyboardInputs[position] = newInput;
    }
    // ---- GETTERS ----------------- //
    public List<string> GetInputsForControl(int control)
    {
        if(control == 0) return _keyboardInputs;
        else return _formatsToGamepad;
    }
    // ---- SETTERS ----------------- //
    public void SetInputsForControl(int control, List<string> values)
    {
        if (control == 0) _keyboardInputs = values;
        else _formatsToGamepad = values;
    }
}
// PARA EDITAR GAMEPAD EL ARRAY DE FORMATOS ESTÁ EN CONSTANTE CAMBIO DE POSICIONES
// PARA EDITAR KEYBOARD EL ARRAY DE FORMATOS NO SE MODIFICA PERO SE CAMBIAN LOS VALORES DEL ARRAY DE INPUTS