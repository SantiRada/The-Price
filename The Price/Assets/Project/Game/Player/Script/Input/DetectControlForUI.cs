using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectControlForUI : MonoBehaviour {

    [Header("Data Control UI")]
    [SerializeField] private Sprite[] _keyboardControl;
    [SerializeField] private Sprite[] _playstationControl;
    [SerializeField] private Sprite[] _xboxControl;

    [Header("Data Players")]
    [SerializeField] private TypeController[] _players;

    [Header("Private Data")]
    private List<Image> _contentKey = new List<Image>();
    private List<string> _contentValue = new List<string>();

    private EditorInputs _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<EditorInputs>();
    }
    private void Start()
    {
        ChangeDetectValues();
    }
    private void LoadAllKeys()
    {
        // TEXT.MESH.PRO
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
    private void ChangeDetectValues()
    {
        LoadAllKeys();

        for(int i = 0; i < _contentKey.Count; i++)
        {
            _contentKey[i].sprite = GetInput(_contentValue[i]);
        }
    }
    public Sprite GetInput(string use)
    {
        Sprite spr = null;

        // Leer _inputs._inputData en la posición en que suceda "use" para saber que tecla devolver
        switch (use)
        {
            case "use": break;
            case "attack": break;
            case "dash": break;
            case "skillOne": break;
            case "skillTwo": break;
            case "stats": break;
            case "staticAim": break;
            case "pause": break;
        }

        return spr;
    }
}
