using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectControlForUI : MonoBehaviour {

    [Header("Data for Controls")]
    [SerializeField] private Sprite _sprForKeyboard;
    [SerializeField] private string[] _dataForGamepad = new string[12];
    [SerializeField] private Sprite[] _sprForXbox = new Sprite[12];
    [SerializeField] private Sprite[] _sprForPlayStation = new Sprite[12];

    [Header("Data Control UI")]
    private Dictionary<string, Sprite> _playStationControl = new Dictionary<string, Sprite>(12);
    private Dictionary<string, Sprite> _xboxControl = new Dictionary<string, Sprite>(12);

    [Header("Data Players")]
    [SerializeField] private TypeController[] _players;

    [Header("Private Data")]
    [SerializeField] private List<Image> _contentKey = new List<Image>();
    [SerializeField] private List<string> _contentValue = new List<string>();

    private EditorInputs _inputs;

    private void Awake()
    {
        _inputs = GetComponent<EditorInputs>();
    }
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
            if (contentImage.name.Contains("Gamepad"))
            {
                _alListImages.Add(contentImage);

                string[] data = contentImage.name.Split("[");
                string[] subdata = data[1].Split("]");

                _contentValue.Add(subdata[0]);
            }
        }

        _contentKey = _alListImages;

        LoadAllInputs();
    }
    private void LoadAllInputs()
    {
        for (int i = 0; i < _inputs._inputActionReference.Length; i++)
        {
            // GAMEPAD FILL
            for (int j = 0; j < _dataForGamepad.Length; j++)
            {
                if (_dataForGamepad[j] == _inputs.gamepadData[i])
                {
                    string[] data = _inputs._inputActionReference[i].name.Split("/");
                    string name = data[1];

                    _xboxControl.Add(name, _sprForXbox[j]);
                    _playStationControl.Add(name, _sprForPlayStation[j]);
                    break;
                }
            }
        }
    }
    public void ChangeDetectValues()
    {
        LoadAllPads();

        for (int i = 0; i < _contentKey.Count; i++)
        {
            Sprite spr = GetInput(_contentValue[i]);

            if (spr != null)
            {
                // GAMEPAD
                _contentKey[i].sprite = spr;
            }
            else
            {
                // KEYBOARD
                _contentKey[i].sprite = _sprForKeyboard;
                TextMeshProUGUI tmKey = _contentKey[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();
                for(int j = 0; j < _inputs._inputActionReference[i].action.bindings.Count; j++)
                {
                    if (_inputs._inputActionReference[i].action.bindings[j].path.Contains("Keyboard"))
                    {
                        string[] data = _inputs._inputActionReference[i].action.bindings[j].path.Split('/');
                        tmKey.text = data[1].ToString();
                        break;
                    }
                }
            }
        }
    }
    public Sprite GetInput(string use)
    {
        if (_players[0] == TypeController.Keyboard) return null;
        else if (_players[0] == TypeController.PlayStation) return _playStationControl[use];
        else return _xboxControl[use];
    }
}
