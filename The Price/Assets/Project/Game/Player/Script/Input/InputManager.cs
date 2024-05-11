using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    [Header("Data for Controls")]
    [SerializeField] private Sprite _sprForKeyboard;
    [SerializeField] private Sprite[] _sprForXbox = new Sprite[12];
    [SerializeField] private Sprite[] _sprForPlayStation = new Sprite[12];

    [Header("Data for Inputs")]
    [SerializeField] private string[] _keyboardInputs = new string[12]; // ARRAY DE INPUTS
    [SerializeField] private string[] _formatsToGamepad = { "use", "attack", "dash", "staticAim", "skillOne", "skillTwo", "select", "pause" };
    [SerializeField] private string[] _formatsToKeyboard = { "use", "attack", "dash", "staticAim", "skillOne", "skillTwo" ,"select", "pause" };

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

                string[] data = contentImage.name.Split("[");
                string[] subdata = data[1].Split("]");

                _contentValue.Add(subdata[0]);
            }
        }

        _contentKey = _alListImages;
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
        for(int i = 0; i < _formatsToKeyboard.Length; i++)
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
        for (int i = 0; i < _formatsToGamepad.Length; i++)
        {
            if (_formatsToGamepad[i] == use)
            {
                // ENCONTRÉ LA POSICION DEL PARAMETRO BUSCADO
                if (element.Contains("xbox")) return _sprForXbox[i];
                else if (element.Contains("board")) return _sprForKeyboard;
                else return _sprForPlayStation[i];
            }
        }

        return null;
    }
}

// PARA EDITAR GAMEPAD EL ARRAY DE FORMATOS ESTÁ EN CONSTANTE CAMBIO DE POSICIONES
// PARA EDITAR KEYBOARD EL ARRAY DE FORMATOS NO SE MODIFICA PERO SE CAMBIAN LOS VALORES DEL ARRAY DE INPUTS