using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveManager : MonoBehaviour {

    [Header("Window")]
    [SerializeField] private List<Vector3> _offset = new List<Vector3>();
    [SerializeField] private List<GameObject> _windowThatWillOpen = new List<GameObject>();

    [Header("Small UI")]
    [SerializeField] private TextMeshProUGUI _descSmall;

    [Header("Big UI")]
    [SerializeField] private TextMeshProUGUI _nameBig;
    [SerializeField] private TextMeshProUGUI _descBig;

    [Header("Private Content")]
    private List<Vector3> _initPos = new List<Vector3>();

    private void Start()
    {
        _initPos.Add(_windowThatWillOpen[0].transform.position);
        _initPos.Add(_windowThatWillOpen[1].transform.position);
    }
    public void LoadBigInfo(int name, int desc, Vector3 pos)
    {
        _windowThatWillOpen[1].transform.position = (pos + _offset[1]);

        _nameBig.text = LanguageManager.GetValue("Game", name);
        _descBig.text = LanguageManager.GetValue("Game", desc);
    }
    public void LoadSmallInfo(int desc, Vector3 pos)
    {
        _windowThatWillOpen[0].transform.position = (pos + _offset[0]);

        _descSmall.text = LanguageManager.GetValue("Game", desc);
    }
    public void CloseWindow()
    {
        for(int i = 0; i < _windowThatWillOpen.Count; i++) { _windowThatWillOpen[i].transform.position = _initPos[i]; }
    }
}
