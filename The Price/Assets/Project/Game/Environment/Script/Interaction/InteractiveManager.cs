using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveManager : MonoBehaviour {

    [Header("Window")]
    public List<Vector3> _offset = new List<Vector3>();
    public List<GameObject> _windowThatWillOpen = new List<GameObject>();
    [Space]
    public TextMeshProUGUI[] _nameContent;
    public TextMeshProUGUI[] _descContent;

    [Header("Style")]
    public string colorNames;

    [Header("Private Content")]
    private List<Vector3> _initPos = new List<Vector3>();

    private void Start()
    {
        for(int i = 0; i < _windowThatWillOpen.Count; i++) { _initPos.Add(_windowThatWillOpen[i].transform.position); }
    }
    public void LoadInfo(int index, int desc, Vector3 pos, int name, ListContent content)
    {
        string cnt = content.ToString();

        _windowThatWillOpen[index].transform.position = (pos + _offset[index]);

        if (index != 2) _nameContent[index].text = LanguageManager.GetValue(cnt, name);

        _descContent[index].text = LanguageManager.GetValue(cnt, desc);

        // AGREGAR NOMBRE DESPUÉS DE "HABLAR CON"
        if(index == 2) _descContent[index].text += " <color=#" + colorNames + ">" + LanguageManager.GetValue(cnt, name) + "</color>";
    }
    public void CloseWindow()
    {
        for(int i = 0; i < _windowThatWillOpen.Count; i++) { _windowThatWillOpen[i].transform.position = _initPos[i]; }
    }
}
